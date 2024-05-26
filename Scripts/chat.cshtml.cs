using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace WebApplication1.Pages
{
    public class ChatModel : PageModel
    {
        private const int MaxBots = 9;
        private const string BotIdCookieName = "BotId";
        private const string ArrayOfListsCookieName = "arrayOfLists";

        public string? botname;

        private OpenAIChatRequest OpenAIObj = new OpenAIChatRequest();

        [BindProperty]
        public string Message { get; set; } = string.Empty;

        private int? _botId;

        
        public int BotId
        {
            get => _botId ??= GetBotIdFromCookie();
            set => SetBotIdInCookie(_botId = value);
            
        }

        

        public List<string>[] ArrayOfLists
        {
            get => GetArrayOfListsFromCookie() ?? InitializeArrayOfLists();
            set => SetArrayOfListsInCookie(value);
        }

        public List<string> CurrentMessages { get; set; }

        public void OnGet(string bot)
        {
            if (int.TryParse(bot, out int botId) && botId.InRange(0, MaxBots))
            {
                BotId = botId;
            }
            SetCurrentMessages();
        }

        public async Task<IActionResult> OnPostSendMessage()
        {
            if (string.IsNullOrWhiteSpace(Message))
                return BadRequest("Message cannot be empty.");

            if (BotId.InRange(0, MaxBots))
            {
                var array = ArrayOfLists;

                array[BotId].Add($"User: {Message}");
                string response = await ProcessMessage(array[BotId], BotId);
                array[BotId].Add($"AI: {response}");

                ArrayOfLists = array;
                SetCurrentMessages();

                return new JsonResult(new { response, currentMessages = CurrentMessages });
            }

            return BadRequest("Invalid bot ID.");
        }

        public IActionResult OnPostDeleteMessage(){
            var array = ArrayOfLists;
            array[BotId].Clear();
            ArrayOfLists = array;
            SetCurrentMessages();
            
            return new JsonResult(new { currentMessages = CurrentMessages });
        }
        

        private void SetCurrentMessages() => CurrentMessages = ArrayOfLists[BotId];

        private int GetBotIdFromCookie() => 
            int.TryParse(Request.Cookies[BotIdCookieName], out int botId) && botId.InRange(0, MaxBots)
                ? botId : 0;

        private void SetBotIdInCookie(int? botId) =>
            Response.Cookies.Append(BotIdCookieName, botId.ToString(), CreateCookieOptions());

        private List<string>[] GetArrayOfListsFromCookie() =>
            Request.Cookies.TryGetValue(ArrayOfListsCookieName, out var cookieData) && !string.IsNullOrWhiteSpace(cookieData)
                ? JsonConvert.DeserializeObject<List<string>[]>(cookieData) : null;

        private void SetArrayOfListsInCookie(List<string>[] array) =>
            Response.Cookies.Append(ArrayOfListsCookieName, JsonConvert.SerializeObject(array), CreateCookieOptions());

        private async Task<string> ProcessMessage(List<string> lista, int botid)
        {
            return await OpenAIObj.SendRequestToOpenAI(lista, botid);
        }

        private static List<string>[] InitializeArrayOfLists() =>
            Enumerable.Range(0, MaxBots).Select(_ => new List<string>()).ToArray();

        private static CookieOptions CreateCookieOptions() => new()
        {
            Expires = DateTime.Now.AddDays(1),
            Path = "/"
        };
    }

    public static class Extensions
    {
        public static bool InRange(this int value, int min, int max) => value >= min && value < max;
    }
}