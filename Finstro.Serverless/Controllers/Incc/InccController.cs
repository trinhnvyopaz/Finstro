using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Finstro.Serverless.Helper;
using Finstro.Serverless.Models;
using Finstro.Serverless.Models.Request;
using Finstro.Serverless.Models.Request.Incc;
using Finstro.Serverless.Models.Response.Incc;
using FinstroServerless.Services.Clients;
using FinstroServerless.Services.Incc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finstro.Serverless.API.Controllers.Incc
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InccController : BaseController
    {
        private readonly InccService inccService = new InccService();

        [Route("Get24HourToken")]
        [HttpPost]
        [Authorize]
        public ActionResult Get24HourToken()
        {
            try
            {
                var token = this.inccService.Get24HourToken();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }


        [Route("CreateCard")]
        [HttpPost]
        [Authorize]
        public ActionResult CreateCard()
        {
            try
            {
                var token = this.inccService.CreateCard(UserId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }

        [Route("Get30MinToken")]
        [HttpPost]
        [Authorize]
        public ActionResult Get30MinToken(GenericRequest cardToken)
        {
            try
            {
                var token = this.inccService.Get30MinToken(UserId, cardToken.Data);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }


        [Route("GetCardData")]
        [HttpPost]
        [Authorize]
        public ActionResult GetCardData(GetCardDataRequest cardData)
        {
            try
            {
                cardData.CallType = EnumInccCardCallType.MaskedData.GetDescription();
                var token = this.inccService.GetCardData(UserId, cardData);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }

        [Route("ActivateCard")]
        [HttpPost]
        [Authorize]
        public ActionResult ActivateCard(GetCardDataRequest cardData)
        {
            try
            {
                this.inccService.ActivateCard(UserId, cardData);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }


        [Route("LockCard")]
        [HttpPost]
        [Authorize]
        public ActionResult LockCard(GetCardDataRequest cardData)
        {
            try
            {
                this.inccService.LockUnlockCard(UserId, cardData, EnumInccCardAction.TemporaryLockCard);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }

        [Route("UnlockCard")]
        [HttpPost]
        [Authorize]
        public ActionResult UnlockCard(GetCardDataRequest cardData)
        {
            try
            {
                this.inccService.LockUnlockCard(UserId, cardData, EnumInccCardAction.UnlockCard);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }


        [Route("PermLockCard")]
        [HttpPost]
        [Authorize]
        public ActionResult PermLockCard(GetCardDataRequest cardData)
        {
            try
            {
                this.inccService.LockUnlockCard(UserId, cardData, EnumInccCardAction.PermanentLock);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }

        /// <summary>
        ///   Get list of current clients.
        /// </summary>
        /// <param name="request">Object carrying the ReplaceCard</param>
        /// <returns></returns>
        [Route("ReplaceCard")]
        [HttpPost]
        public ActionResult ReplaceCard(CardActionRequest request)
        {
            try
            {
                request.UserId = UserId;
                this.inccService.ReplaceCard(request);

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }

    }
}