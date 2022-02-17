using MailCore.Controllers;
using MailCore.Infrastructure;
using MailCore.Log;
using MailCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MonqLabTest.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class MailsController : ControllerBase
    {
        /// <summary>
        /// Обработка POST запроса
        /// </summary>
        /// <param Параметры сообщения="message"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Post(Message message)
        {
            try
            {
                var controller = new LoggerController(new ControllerBuilder()
                                              .SetModel(new Model(new Sending(), message))
                                              .Build(), new DatabaseLog());
                controller.SendMessage();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        /// <summary>
        /// Возвращение записей об отправке сообщений
        /// </summary>
        /// <returns>JSON объекты</returns>
        [HttpGet]
        public ActionResult<object> Get()
        {
            try
            {
                using (SendingContext context = new SendingContext())
                {
                    var records = context.RecordsSending.Join(context.Messages,
                                                                                    record => record.MessageId,
                                                                                    message => message.Id,
                                                                                    (record, message) => new
                                                                                    {
                                                                                        DateSending = record.timeSending,
                                                                                        Result = record.Result,
                                                                                        Status = record.FailedMessage,
                                                                                        Subject = message.Subject,
                                                                                        Body = message.Body,
                                                                                        Recipients = message.Recipients
                                                                                    }).ToArray();
                    return Ok(records);
                }
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
