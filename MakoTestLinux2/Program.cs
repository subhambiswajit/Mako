using System;
using System.IO;
using JawsMako;

namespace Valkyrie.Conversion
{
    public static class Program
    {
        private static void Main()
        {
            using var mako = IJawsMako.create();
            IJawsMako.enableAllFeatures(mako);

            var directory = new DirectoryInfo("TestFiles");
            var files = directory.EnumerateFiles();

            foreach (var file in files)
            {
                var inputFormat = GetFormat(file.Extension);
                using var input = IInput.create(mako, inputFormat);

                using var assembly = input.open(file.FullName);

                using var page = assembly.getDocument().getPage().edit();

                using var black = IDOMColor.createFromArray(mako.getFactory(), IDOMColorSpaceDeviceRGB.create(mako.getFactory()), 0.15f, new[] { 0f, 0f, 0f });
                using var brush = IDOMSolidColorBrush.create(mako.getFactory(), black);

                using var font = mako.findFont("Calibri Light", out var fontIndex);
                using var glyphs = IDOMGlyphs.create(mako.getFactory(), "Watermarked", 120, new FPoint(50d, 300d), brush, font, fontIndex);

                page.appendChild(glyphs);

                using var output = IPDFOutput.create(mako);
                output.writeAssembly(assembly, $"{file.Name}.pdf");
            }
        }

        private static eFileFormat GetFormat(string fileExtension)
        {
            return fileExtension.ToLower() switch
            {
                ".pdf" => eFileFormat.eFFPDF,
                ".xps" => eFileFormat.eFFXPS,
                ".ps" => eFileFormat.eFFPS,
                ".pcl" => eFileFormat.eFFPCL5,
                ".pxl" => eFileFormat.eFFPCLXL,

                _ => throw new InvalidOperationException($"Unknown file extension: {fileExtension}")
            };
        }
    }
}