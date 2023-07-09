using OmegaPoint.Integration.Replicate;
using OmegaPoint.Integration.Replicate.Models;
using ShapeCrawler;

namespace OmegaPoint;

internal class Program
{
    static async Task Main(string[] args)
    {
        var replicateToken = args[0];
        var fileName = args[1];
        var photosFolder = args[2];
        var prompt = args[3];

        IPresentation pres;

        if (File.Exists(fileName)) {
            pres = SCPresentation.Open(fileName);
        } else {
            pres = SCPresentation.Create();

            //Uncomment for intro slide
            // var shapes = pres.Slides[0].Shapes;
            // var textShape = shapes.AddRectangle(0, 0, pres.SlideWidth, pres.SlideHeight);
            // textShape.TextFrame!.Text = "This is an intro slide";
        }

        var layout = pres.SlideMasters[0].SlideLayouts[0];

        var replicateApi = new ReplicateApi(new()
        {
            ApiBaseUrl = "https://api.replicate.com/",
            ApiToken = replicateToken
        });

        var replicateService = new ReplicateService(new HttpClient(), replicateApi);

        var random = new Random();
        foreach (var file in Directory.GetFiles(photosFolder).OrderBy(x => random.Next()))
        {
            try
            {
                Console.WriteLine($"Adding {file} to presentation");
                using var img = SixLabors.ImageSharp.Image.Load(file);
                using var imgStream = File.OpenRead(file);

                Console.WriteLine($"Getting caption for {file}");

                var shapes = pres.Slides.AddEmptySlide(layout).Shapes;

                //add image to slide
                var picShape = shapes.AddPicture(imgStream);

                //get aspect ratio and scale image to fit slide
                double ratioX = pres.SlideWidth / (double)img.Width;
                double ratioY = pres.SlideHeight / (double)img.Height;
                double ratio = Math.Min(ratioX, ratioY);

                picShape.Width = (int)(img.Width * ratio);
                picShape.Height = (int)(img.Height * ratio);

                picShape.X = 0;
                picShape.Y = 0;

                //center image on slide
                if (ratioX > ratioY)
                    picShape.X = (int)((pres.SlideWidth - img.Width * ratio) / 2);
                else
                    picShape.Y = (int)((pres.SlideHeight - img.Height * ratio) / 2);

                ScaleForReplicate(img);
                var imgString = img.ToBase64String(img.Metadata.DecodedImageFormat);

                var captionResult = await replicateService.GetPrediction<CaptionRequest, string>("b96a2f33cc8e4b0aa23eacfce731b9c41a7d9466d9ed4e167375587b54db9423", new CaptionRequest()
                {
                    Image = imgString,
                    Prompt = prompt,
                    Temperature = 1.8
                });

                //add caption to slide
                if (captionResult.HasValue && captionResult.Value.Output != null)
                {
                    // var textShape = shapes.AddRectangle(0, 0, pres.SlideWidth, pres.SlideHeight);
                    // textShape.TextFrame!.Text = captionResult.Value.Output.Title;

                    var textShape = shapes.AddRectangle(0, 0, pres.SlideWidth, pres.SlideHeight);
                    textShape.TextFrame!.Text = captionResult.Value.Output.Replace("\n", string.Empty);
                    var textPortion = textShape.TextFrame!.Paragraphs[0].Portions[0];
                    textPortion.TextHighlightColor = ShapeCrawler.Drawing.SCColor.White;
                }

                Console.WriteLine("Saving presentation");
                pres.SaveAs(fileName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error opening or writing file: {0}", e.Message);
            }
        }
    }

    private static void ScaleForReplicate(Image img)
    {
        double ratioX = 800 / (double)img.Width;
        double ratioY = 800 / (double)img.Height;
        double ratio = Math.Min(ratioX, ratioY);

        var newWidth = (int)(img.Width * ratio);
        var newHeight = (int)(img.Height * ratio);

        img.Mutate(x => x.Resize(newWidth, newHeight));
    }
}