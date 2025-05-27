using GemiNet;

var ai = new GoogleGenAI();

var response = await ai.Models.GenerateContentAsync(new()
{
    Model = Models.Gemini1_5Flash,
    Contents = "Hello, Gemini!"
});

Console.WriteLine(response.GetText());

await foreach (var model in ai.Models.ListAsync())
{
    Console.WriteLine(model.Name);
}