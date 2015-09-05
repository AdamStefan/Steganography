using MathLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MathLibrary.Matrices;
using System.Linq;
using System.IO;

namespace ImageHelpers
{
    public class WatermarkProcessor
    {

        private static string _debugEncodeTextFile = "d:\\encodeText.txt";
        private static string _debugDecodeTextFile = "d:\\decodeText.txt";

        private static  StringBuilder _debugEncodeStringBuilder = new StringBuilder();
        private static  StringBuilder _debugDecodeStringBuilder = new StringBuilder();
        #region Encode

        public static void WatermarkDctWithLsb(string sourceFile, string destinationFile, string secretmessage, int bitIndex = 0 , ChannelType channelType = ChannelType.Y)
        {
            var bitmap = (Bitmap)Image.FromFile(sourceFile);             
            var ret = WatermarkDctSingleBitPerBlock(bitmap, Encoding.ASCII.GetBytes(secretmessage), bitIndex, channelType);
            JpegUtilitites.Save((Bitmap)ret, destinationFile, JpegUtilitites.GetQuantizationTableArray(bitmap, QuantizationTableType.Luminance), JpegUtilitites.GetQuantizationTableArray(bitmap, QuantizationTableType.Chrominance));            
        }

        public static void WatermarkDctWithLsb(string sourceFile, string destinationFile, byte[] secretmessage, int bitIndex = 0, ChannelType channelType = ChannelType.Y)
        {
            var bitmap = (Bitmap)Image.FromFile(sourceFile);
            var ret = WatermarkDctSingleBitPerBlock(bitmap, secretmessage, bitIndex, channelType);
            JpegUtilitites.Save((Bitmap)ret, destinationFile, JpegUtilitites.GetQuantizationTableArray(bitmap, QuantizationTableType.Luminance), JpegUtilitites.GetQuantizationTableArray(bitmap, QuantizationTableType.Chrominance));
        }

        /// <summary>
        /// Watermark an image using DCt transform
        /// </summary>
        /// <param name="inputImage">the input image usually a jpeg image</param>
        /// <param name="secretmessage">the hidden message</param>
        /// <param name="bitIndex">the byte index from the block in wich the message will be added</param>
        /// <param name="channelType"></param>
        /// <returns></returns>
        public static Image WatermarkDctSingleBitPerBlock(Bitmap inputImage, byte[] secretmessage, int bitIndex, ChannelType channelType)
        {
            var image = ImageHelpers.ImageToArray(inputImage);

            var messageLength = (UInt16)secretmessage.Length;
            var messageToEmbedd = BitConverter.GetBytes(messageLength).Concat(secretmessage).ToArray();

            
            var embeddingSecretMessageBitIndex = 0;
            bool canEmbed = embeddingSecretMessageBitIndex < messageToEmbedd.Length*8;

            var zigzagAccessor = new ZigZagAccessorFactory<double>(8, 8);

            if (!canEmbed)
            {
                return new Bitmap(inputImage);
            }
            QuantizationTableType quantizationType = channelType == ChannelType.Y ? QuantizationTableType.Luminance : QuantizationTableType.Chrominance;

            var quantizationTable = JpegUtilitites.GetQuantizationTable(inputImage, quantizationType);
            var imageBlocks = MathUtilities.Split2DArrayToBlocks(image, 8);

            var stegoBlocks = new List<MatrixBase<Color>>();

            for (var index = 0; index < imageBlocks.Count; index++)
            {
                var imageBlock = imageBlocks[index];
                var block = WatermarkDctBlock(imageBlock, zigzagAccessor, quantizationTable, canEmbed, messageToEmbedd, ref embeddingSecretMessageBitIndex, bitIndex, channelType);
                canEmbed = embeddingSecretMessageBitIndex < messageToEmbedd.Length * 8;
                stegoBlocks.Add(block);
            }

            File.WriteAllText(_debugEncodeTextFile, _debugEncodeStringBuilder.ToString());

            var stegoImageArray = MathUtilities.Merge2DArraysBlocks(stegoBlocks, inputImage.Height, inputImage.Width);                  

            var stegoImage = ImageHelpers.ArrayToImage(stegoImageArray);

            return stegoImage;
        }


        private static MatrixBase<Color> WatermarkDctBlock(MatrixBase<Color> block, ZigZagAccessorFactory<Double> accessorFactory, double[,] quantizationTable, bool canEncrypt,
                                                           byte[] secretMessage,
                                                           ref int embeddingSecretMessageBitIndex, int byteIndexForEncryption, ChannelType channelType)
        {

            if (!canEncrypt)
            {                
                return block;
            }

            var byteIndex = embeddingSecretMessageBitIndex / 8;
            if (embeddingSecretMessageBitIndex % 8 == 0)
            {
                byteIndex = embeddingSecretMessageBitIndex / 8;
                _debugEncodeStringBuilder.AppendLine("Appending byte:" + (byteIndex + 1));
            }


            var correction = 1;
            bool embeddCorrectly;
            MatrixBase<Color> ret;

            //#region Convert block 

            var yCbCrColorBlock = block.ToYCbCr();
            var channel = yCbCrColorBlock.GetChannel(channelType);
            channel = channel - 128;
            DoubleMatrix dctBlockTransform = null;

            //#endregion

            do
            {                
                // For luminance channel yMax = 127, ymin = -128

                DoubleMatrix stegoBlock;
                bool redo = false;
                var nextSecretBit = MathUtilities.GetBit(secretMessage, embeddingSecretMessageBitIndex);
                bool changed = false;
                dctBlockTransform = DiscreteCosineTransform.ForwardDct8Block(channel);
                dctBlockTransform = (dctBlockTransform.CrossDivide(quantizationTable));
                dctBlockTransform = dctBlockTransform.Round();                

                var accessor = accessorFactory.Access(dctBlockTransform);
                var curentElelement = accessor[byteIndexForEncryption];
                bool isEven = ((int) Math.Round(curentElelement))%2 == 0;
                double possibleValue = curentElelement;
                DoubleMatrix oldStegoBlock = null;

                do
                {
                    if ((isEven && nextSecretBit) || (!isEven && !nextSecretBit))
                    {
                        if (!redo)
                        {
                            possibleValue = isEven ? curentElelement - correction : curentElelement + correction;
                        }
                        else
                        {
                            possibleValue = isEven ? curentElelement + correction : curentElelement - correction;
                        }
                        changed = true;
                    }

                    accessor[byteIndexForEncryption] = possibleValue;
                    stegoBlock = dctBlockTransform.CrossProduct(quantizationTable);
                    stegoBlock = DiscreteCosineTransform.InverseDct8Block(stegoBlock);

                    if (!redo && changed)
                    {
                        for (var i = 0; i < 8; i++)
                        {
                            for (var j = 0; j < 8; j++)
                            {
                                var value = Math.Round(stegoBlock[i, j]);
                                if (value < -128 || value > 127)
                                {
                                    redo = true;
                                    oldStegoBlock = stegoBlock;
                                    break;
                                }
                            }
                            if (redo)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (changed)
                        {
                            var maximumValue = oldStegoBlock.MaximumValue;
                            var minimumValue = oldStegoBlock.MinimumValue;
                            var currentMaximumValue = stegoBlock.MaximumValue;
                            var currentMinimumValue = stegoBlock.MinimumValue;

                            if (currentMinimumValue < -128 || currentMaximumValue > 127)
                            {
                                if ((currentMinimumValue < -128 && minimumValue > currentMinimumValue) || (currentMaximumValue > 127 && maximumValue < currentMaximumValue))
                                {
                                    stegoBlock = oldStegoBlock;
                                }
                            }
                       } 
                        redo = false;
                    }
                } while (redo);

                
                stegoBlock = stegoBlock + 128; // 
                var tmpyCbCrColorBlock = yCbCrColorBlock.Copy;
                tmpyCbCrColorBlock.UpdateChannel(channelType, stegoBlock);

                ret = tmpyCbCrColorBlock.ToRgb();

                #region Verification (check if embeddding is correctly (rounding problems might occurs))

                if (changed)
                {
                    embeddCorrectly = !GetWatermarkDctForBlock(ret, byteIndexForEncryption, accessorFactory, quantizationTable, channelType) == nextSecretBit;
                    if (!embeddCorrectly)
                    {
                        correction = correction + 2;
                    }
                    else
                    {
                        correction = 1;
                    }
                }
                else
                {
                    embeddCorrectly = true;
                    correction = 1;
                }

                #endregion

            } while (!embeddCorrectly);

           
            _debugEncodeStringBuilder.AppendLine(dctBlockTransform.ToString());
            embeddingSecretMessageBitIndex++;

            return ret;
        }

        #endregion       

        #region Decode

        public static bool GetWatermarkDctForBlock(MatrixBase<Color> block, int bitIndex, ZigZagAccessorFactory<double> zigzagAccessorFactory, Double[,] quantizationTable,
                                                   ChannelType channelType, bool log = false)
        {
            var yCbCrColorBlock = block.ToYCbCr();
            var yChannel = yCbCrColorBlock.GetChannel(channelType);
            var shiftetBlock = yChannel - 128;
            var dctBlockTransform = (DiscreteCosineTransform.ForwardDct8Block(shiftetBlock).CrossDivide(quantizationTable)).Round();
            if (log)
            {
                _debugDecodeStringBuilder.AppendLine(dctBlockTransform.ToString());
            }
            var accessor = zigzagAccessorFactory.Access(dctBlockTransform);
            var coefficient = accessor[bitIndex];
            var curentElelement = (int) Math.Round(coefficient);
            bool isEven = curentElelement%2 == 0;
            return isEven;
        }

        public static byte[] GetWaterMarkDct(Bitmap stegoImage, int bitIndex, ChannelType channelType = ChannelType.Y)
        {
            var imageColorContent = ImageHelpers.ImageToArray(stegoImage);

            int currentBitIndex = 0;
            var byteList = new List<Byte>();
            byte currentByte = 0;
            QuantizationTableType quantizationType = channelType == ChannelType.Y ? QuantizationTableType.Luminance : QuantizationTableType.Chrominance;
            var zigzagAccessor = new ZigZagAccessorFactory<double>(8, 8);

            var imageBlocks = MathUtilities.Split2DArrayToBlocks(imageColorContent, 8);
            var quantizationTable = JpegUtilitites.GetQuantizationTable(stegoImage, quantizationType);

            bool headerLoaded = false;
            int activeContentLength = imageBlocks.Count*8;
            int byteAppended=0;

            for (int index = 0; index < imageBlocks.Count; index++)
            {

                _debugDecodeStringBuilder.AppendLine("Appending byte:" + (byteAppended +1));
                var imageBlock = imageBlocks[index];
                bool isEven = GetWatermarkDctForBlock(imageBlock, bitIndex, zigzagAccessor, quantizationTable, channelType, true);                

                MathUtilities.AddBit(ref currentByte, !isEven);
                if (currentBitIndex%8 == 7)
                {
                    byteList.Add(currentByte);

                    byteAppended++;
                    if (!headerLoaded && byteList.Count == 2)
                    {
                        activeContentLength = BitConverter.ToUInt16(byteList.ToArray(), 0);
                        headerLoaded = true;
                        byteList.Clear();
                    }
                    currentByte = new byte();
                }
                currentBitIndex++;
                if (byteList.Count == activeContentLength)
                {
                    break;
                }
            }

            File.WriteAllText(_debugDecodeTextFile, _debugDecodeStringBuilder.ToString());
            return byteList.ToArray();
        }

        public static string GetTextWatermarkDct(Bitmap stegoImage, int bitIndex, ChannelType channelType = ChannelType.Y)
        {
            var bytes = GetWaterMarkDct(stegoImage, bitIndex, channelType);
            return Encoding.ASCII.GetString(bytes);
        }

        #endregion        

    }
}
