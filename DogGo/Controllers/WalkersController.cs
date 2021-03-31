using DogGo.Models;
using DogGo.Repositories;
using DogGo.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace DogGo.Controllers
{
    public class WalkersController : Controller
    {
        private readonly IWalkerRepository _walkerRepo;
        private readonly IWalkRepository _walkRepo;
        private readonly IDogRepository _dogRepo;
        private readonly IOwnerRepository _ownerRepo;

        // ASP.NET will give us an instance of our Walker Repository. This is called "Dependency Injection"
        public WalkersController(IWalkerRepository walkerRepository,
                                IWalkRepository walkRepository,
                                IDogRepository dogRepository,
                                IOwnerRepository ownerRepository)
        {
            _walkerRepo = walkerRepository;
            _walkRepo = walkRepository;
            _dogRepo = dogRepository;
            _ownerRepo = ownerRepository;
        }

        // GET: WalkersController
        public ActionResult Index()
        {
            int ownerId = GetCurrentUserId();
            
            if(ownerId != 0)
            {
                Owner owner = _ownerRepo.GetOwnerById(ownerId);
                List<Walker> walkersByNeighborhood = _walkerRepo.GetWalkersInNeighborhood(owner.NeighborhoodId);

                return View(walkersByNeighborhood);
            }

            List<Walker> walkers = _walkerRepo.GetAllWalkers();

            return View(walkers);
        }

        // GET: Walkers/Details/5
        public ActionResult Details(int id)
        {
            Walker walker = _walkerRepo.GetWalkerById(id);
            List<Walk> walks = _walkRepo.GetWalksByWalkerId(id);

            WalkerProfileViewModel vm = new WalkerProfileViewModel()
            {
                Walker = walker,
                Walks = walks
            };

            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        // GET WalkersController/CreateWalk
        public ActionResult CreateWalk(int id)
        {
            int ownerId = GetCurrentUserId();
            Walker walker = _walkerRepo.GetWalkerById(id);
            List<Dog> dogs = _dogRepo.GetDogsByOwnerId(ownerId);

            WalkFormViewModel vm = new WalkFormViewModel()
            {
                Walk = new Walk() 
                {
                    WalkerId = walker.Id,
                    Duration = 0
                },
                Walks = new List<Walk>(),
                Walker = walker,
                Dogs = dogs,
            };

            return View(vm);
        }

        // POST: WalkersController/CreateWalk
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWalk(WalkFormViewModel walkFormViewModel)
        {
            try
            {
                foreach (int dogId in walkFormViewModel.DogIds)
                {
                    Walk walk = new Walk()
                    {
                        Date = walkFormViewModel.Walk.Date,
                        Duration = walkFormViewModel.Walk.Duration,
                        WalkerId = walkFormViewModel.Walk.WalkerId,
                        DogId = dogId
                    };

                    _walkRepo.AddWalk(walk);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View(walkFormViewModel);
            }
        }

        // GET: WalkersController/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: WalkersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: WalkersController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: WalkersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: WalkersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: WalkersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private int GetCurrentUserId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(id))
            {
                return 0;
            }
            else
            {
                return int.Parse(id);
            }

        }
    }
}
