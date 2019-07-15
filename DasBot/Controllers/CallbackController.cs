using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using VkNet.Utils;

namespace DasBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallbackController : ControllerBase
    {
        readonly IConfiguration _configuration;

        readonly IVkApi _vkApi;

        public CallbackController(IVkApi vkApi, IConfiguration configuration)
        {
            _vkApi = vkApi;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Callback([FromBody] Updates updates)
        {
            switch (updates.Type)
            {
                case "confirmation":
                    return Ok(_configuration["Config:Confirmation"]);
                case "message_new":
                    {
                        var msg = Message.FromJson(new VkResponse(updates.Object));
                        string result;
                        var words = msg.Text.Split(' ').Where(word => word.Length > 0).ToArray();
                        if (words.Length < 2)
                            result = "Мне кажется, тут не 2 слова...";
                        else if (words.Length > 2)
                            result = "Мне кажется, что тут больше двух слов...";
                        else
                        {
                            var d = App_Code.Distance.ModifyedLevensteinDistance(words[0], words[1]);
                            if (d == 0)
                                result = "Эти два слова одинаковы!";
                            else if (d <= 0.25)
                                result = "Эти два слова очень похожи.";
                            else if (d <= 0.40)
                                result = "Эти два слова чем-то похожи.";
                            else
                                result = "Эти два слова не похожи";
                        }
                        _vkApi.Messages.Send(new MessagesSendParams
                        {
                            RandomId = new DateTime().Millisecond,
                            PeerId = msg.PeerId.Value,
                            Message = result
                        });
                        break;
                    }
            }
            return Ok("ok");
        }
    }
}
