using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        public void Convert()
        {
            FixImageOrientation(@"C:\\Users\\v.dezhurnyuk\\Pictures\\AI_Foto\man1_1.jpeg", "C:\\Users\\v.dezhurnyuk\\Pictures\\AI_Foto\\567.jpg");
        }

        public static void FixImageOrientation(string inputPath, string outputPath)
        {
            using (var image = Image.Load(inputPath))
            {
                // Попытка получить значение ориентации из EXIF
                if (image.Metadata.ExifProfile != null &&
                    image.Metadata.ExifProfile.TryGetValue(ExifTag.Orientation, out var orientationValue))
                {
                    ushort orientation = orientationValue.Value;

                    switch (orientation)
                    {
                        case 3: // Поворот на 180 градусов
                            image.Mutate(x => x.Rotate(RotateMode.Rotate180));
                            break;

                        case 6: // Поворот на 90 градусов по часовой стрелке
                            image.Mutate(x => x.Rotate(RotateMode.Rotate90));
                            break;

                        case 8: // Поворот на 270 градусов против часовой стрелки
                            image.Mutate(x => x.Rotate(RotateMode.Rotate270));
                            break;
                    }

                    // Удаляем EXIF Orientation, чтобы предотвратить повторное применение
                    image.Metadata.ExifProfile.RemoveValue(ExifTag.Orientation);

                    // Сохраняем изображение
                    image.Save(outputPath);
                    Console.WriteLine("Ориентация исправлена и изображение сохранено.");
                }
                else
                {
                    Console.WriteLine("EXIF Orientation не найден. Изображение не изменено.");
                }
            }
        }
    }
}
