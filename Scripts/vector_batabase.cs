namespace vectordata;
using Microsoft.KernelMemory;

public  class DataProcessing
{

    public  MemoryServerless memory;

    public List<string> documentids;


    public DataProcessing()
    {
        memory =  new KernelMemoryBuilder()
        .WithOpenAIDefaults(DotNetEnv.Env.GetString("OPENAI_API_KEY"))
        .Build<MemoryServerless>();
        documentids = new List<string>();
    }

    public  async Task LoadDock(string path)
    {
        string name = Path.GetFileName(path);
        documentids.Add(await memory.ImportDocumentAsync(path, tags: new() {{ name }}));
    }
    public  async Task DeleteDock()
    {
        foreach (string s in documentids){
            await this.memory.DeleteDocumentAsync(s);
        }
        
    }

    public  async Task<string> ProcessMessage(string messages){
        
        Console.WriteLine(messages);
        var answer1 = await memory.AskAsync(messages);

        foreach (var x in answer1.RelevantSources)
        {
            Console.WriteLine($"  * {x.SourceName} -- {x.Partitions.First().LastUpdate:D}");
        }

        return answer1.Result + " " + answer1.RelevantSources.First().SourceName;
    }

}

