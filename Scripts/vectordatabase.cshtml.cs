using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using vectordata;



   public class VectorDatabase : PageModel
   {

       [BindProperty]
        public FileUpload FileUpload { get; set; }

        public DataProcessing dataprocess = new DataProcessing();

        public List<string> fileEntries = new List<string>();

        private const string ArrayOfListsCookieName = "arrayOfLists";

        public List<string> CurrentMessages { get; set; }


        public List<string>[] ArrayOfLists
        {
            get => GetArrayOfListsFromCookie() ?? InitializeArrayOfLists();
            set => SetArrayOfListsInCookie(value);
        }


       public async void OnGet()
       {
            if (HttpContext.Session.GetString("username") != null){
              string name = HttpContext.Session.GetString("username");
    
              string folderPath = Path.Combine("data", name);
    
              if (Directory.Exists(folderPath))
              {
                string[] files = Directory.GetFiles(folderPath);
                
                foreach (string file in files)
                {
                     fileEntries.Add(Path.GetFileName(file));
                     await dataprocess.LoadDock(file);
                }
              }
         }
       }



       public async Task<IActionResult> OnPostSendMessage(string Message)
        {
            if (string.IsNullOrWhiteSpace(Message)){
                return BadRequest("Message cannot be empty.");
            }
            
                var array = ArrayOfLists;

                array[8].Add($"User: {Message}");
                string response = await dataprocess.ProcessMessage(array[8][^1]);
                array[8].Add($"AI: {response}");

                ArrayOfLists = array;
                SetCurrentMessages();

                return new JsonResult(new { response, currentMessages = CurrentMessages });
            
        }

        

        public IActionResult OnPostDeleteMessage(){

            var array = ArrayOfLists;
            array[8].Clear();
            ArrayOfLists = array;
            SetCurrentMessages();
            
            return new JsonResult(new { currentMessages = CurrentMessages });
        }

        private void SetCurrentMessages() => CurrentMessages = ArrayOfLists[8];

       public async Task<IActionResult> OnPostUploadAsync()
       {
          
           if (!ModelState.IsValid || fileEntries.Count > 5)
           {
               return Page();
           }
      
            string name = HttpContext.Session.GetString("username");

            string folderPath = Path.Combine("data", name);


            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, FileUpload.FormFile.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await FileUpload.FormFile.CopyToAsync(stream);
               
            }
            await dataprocess.LoadDock(filePath);
            string[] files = Directory.GetFiles(folderPath);
                
            foreach (string file in files)
            {
                fileEntries.Add(Path.GetFileName(file));
            }

           return Page();
       }

       public async  Task<IActionResult> OnPostDeleteFiles(){
            string name = HttpContext.Session.GetString("username");
            
            string folderPath = Path.Combine("data", name);
            await dataprocess.DeleteDock();
            fileEntries.Clear();

                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                }
        
            return Page();
            
       }

       
   private List<string>[] GetArrayOfListsFromCookie() =>
            Request.Cookies.TryGetValue(ArrayOfListsCookieName, out var cookieData) && !string.IsNullOrWhiteSpace(cookieData)
                ? JsonConvert.DeserializeObject<List<string>[]>(cookieData) : null;

    private void SetArrayOfListsInCookie(List<string>[] array) =>
            Response.Cookies.Append(ArrayOfListsCookieName, JsonConvert.SerializeObject(array), CreateCookieOptions());

            private static List<string>[] InitializeArrayOfLists() =>
            Enumerable.Range(0, 9).Select(_ => new List<string>()).ToArray();

        private static CookieOptions CreateCookieOptions() => new()
        {
            Expires = DateTime.Now.AddDays(1),
            Path = "/"
        };

   }


   public class FileUpload

   {
       [Required]
       [Display(Name = "File")]
       public IFormFile FormFile { get; set; }
   }

