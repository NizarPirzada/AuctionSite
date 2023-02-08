using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AuctionSite.DAL;
using AuctionSite.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using SuiteShared.DAL.Enums;

namespace AuctionSite.Controllers
{
    [RequireHttps]
    public class imgsController : Controller
    {
        private repo repo;

        public imgsController(repo repo) => this.repo = repo;

        public IActionResult GetAuctionImgs(Guid Id) => Ok(repo.GetImagesPath(Id));

        public IActionResult GetAuctionImage(Guid AuctionId)
        {
            var auction = repo.GetAuctionInformation(AuctionId);
            if (auction == null || auction.PropertyDocuments.All(p => p.DocumentType != DocumentType.Image))
                return new FileStreamResult(new FileStream(@"C:\Scrapped Files\noimg.png", FileMode.Open), "image/png");
            return GetImage(auction.PropertyDocuments.First(p => p.DocumentType == DocumentType.Image).Id);
            //return null;
        }

        public IActionResult GetImage(Guid ImageId)
        {
            var path = repo.GetImagePath(ImageId);
            if (string.IsNullOrEmpty(path)) return BadRequest("No such image where found");
            if (string.IsNullOrEmpty(path)) return new FileStreamResult(new FileStream(@"C:\Scrapped Files\noimg.png", FileMode.Open), "image/png");
            return new FileStreamResult(new FileStream(path, FileMode.Open), "image/jpeg");
        }

        public IActionResult GetAuctionThumb(Guid AuctionId)
        {
            var auction = repo.GetAuctionInformation(AuctionId);
            if (auction == null || auction.PropertyDocuments.All(p => p.DocumentType != DocumentType.Image))
                return new FileStreamResult(new FileStream(@"C:\Scrapped Files\thumb\noimg.png", FileMode.Open), "image/png");
            return GetImage(auction.PropertyDocuments.First(p => p.DocumentType == DocumentType.Image).Id);
        }

        public IActionResult GetImageThumb(Guid ImageId)
        {
            var path = repo.GetImagePath(ImageId);
            if (string.IsNullOrEmpty(path)) return BadRequest("No such image where found");
            if (string.IsNullOrEmpty(path)) return new FileStreamResult(new FileStream(@"C:\Scrapped Files\thumb\noimg.png", FileMode.Open), "image/png");
            return new FileStreamResult(new FileStream(Path.Combine(Path.GetDirectoryName(path), "thumb", Path.GetFileName(path)), FileMode.Open), "image/jpeg");
        }
    }
}
