﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using ReactionRoullete.Services;
using ReactionRoullete.Models;

namespace ReactionRoullete.Controllers
{
    public class DefaultController : Controller
    {
        private readonly YoutubeService youtubeService;
        private readonly ApplicationDbContext db;


        public DefaultController(YoutubeService youtubeService, ApplicationDbContext db)
        {
            this.youtubeService = youtubeService;
            this.db = db;
   
        }
        public async Task< IActionResult> Index()
        {
            //var videos = from x in await youtubeService.GetYoutubeVideoDescriptions()
            //             select x;



            var videoDescriptions = from x in db.YoutubeVideoDescriptions
                                    select x;
            return View(videoDescriptions);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult React(long youtubeVideoDescriptionID)
        {


            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}