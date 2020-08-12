using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using Test.Application.Areas.Identity.Data;
using Test.Application.DTOs;
using Test.Application.SignalR.Hubs;
using Test.Application.SignalR.Services;
using Test.Business.Entities;
using Test.Data.Concurrency;
using Test.Data.Repository;

namespace Test.Application.Controllers
{
    public class EventsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly EventRepository _repository;       
        private readonly UserManager<User> _userManager;
        protected readonly IHttpContextAccessor _httpContext;
        private readonly LockRepository _lockRepository;      
        private readonly SignalREventService _signalREventService;
        private readonly string _entityName = "Event";


        public EventsController(EventRepository repository, IMapper mapper, IHubContext<EventHub> hubContext, UserManager<User> userManager, LockRepository lockManager, SignalREventService signalREventService, IHttpContextAccessor httpContext)
        {
            _repository = repository;
            _mapper = mapper;
            _userManager = userManager;
            _lockRepository = lockManager;
            _signalREventService = signalREventService;
            _httpContext = httpContext;
        }

        // GET: Events
        public IActionResult Index()
        {
            var eventDtoList = _mapper.Map<List<EventDTO>>(_repository.GetAll());
            var lockedList = _lockRepository.GetAllLocks();

            for (int i = 0; i < lockedList.Count; i++)

                foreach (Lock @lock in lockedList)
                {
                    if (!(@lock.AcquiredDateTime <= DateTime.Now.AddSeconds(-15)))
                    {
                        eventDtoList.Where(e => e.Id == @lock.EntityId)
                    .Select(e => { e.IsLocked = true; return e; }).ToList();
                    }

                }

            return View(eventDtoList);
        }

        // GET: Events/Details/5
        public IActionResult Details(int id)
        {          

            EventDTO obj = _mapper.Map<EventDTO>(_repository.GetEventById(id));

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EventDTO obj)
        {
            if (ModelState.IsValid)
            {
                _repository.Add(_mapper.Map<Event>(obj));
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View();
            }

        }

        // GET: Events/Edit/5
        public IActionResult Edit(int id)
        {
            string _currentUser = _httpContext.HttpContext.User.Identity.Name;
            //string _user = _userManager.GetUserName(this.User);            

            try
            {
                EventDTO obj = _mapper.Map<EventDTO>(_repository.GetByIdForEdit(id, _entityName, _currentUser));
                obj.OwnsTheLock = true;
                _signalREventService.InformLockAcquired(id);
                
                return View(obj);
            }
            catch (ConcurrencyException ex)
            {
                EventDTO obj = _mapper.Map<EventDTO>(_repository.GetEventById(id)); // ????????????????
                TempData["ConcurrencyError"] = ex.Message;
                return View(obj);
            }          

        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, EventDTO obj, string cancel, string notify)
        {
            string _user = _userManager.GetUserName(this.User);
            string _groupName = $"{_entityName}-{id}";

            if (!string.IsNullOrEmpty(cancel))
            {
                if (_lockRepository.HasLock(obj.Id, _entityName, _user))
                {                    
                    _lockRepository.ReleaseLock(obj.Id, _entityName, _user);
                    _signalREventService.InformLockReleased(_groupName, id, _user);
                }                 

                return RedirectToAction("index");
            }

            if (!string.IsNullOrEmpty(notify))
            {
                TempData["Notification"] = "You will be notified as soon as the user finish working on this document.";
                return View(obj);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _repository.Update(_mapper.Map<Event>(obj), _user);
                    _signalREventService.InformLockReleased(_groupName, id, _user);                    

                    return RedirectToAction(nameof(Index));
                }
                catch (ConcurrencyException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(obj);
                }

            }
            return View(obj);
        }
        
        // GET: Events/Delete/5
        public IActionResult Delete(int id)
        {
            try
            {
                EventDTO obj = _mapper.Map<EventDTO>(_repository.GetByIdForEdit(id, _entityName, _userManager.GetUserName(this.User)));
                if (obj == null)
                {
                    return NotFound();
                }

                return View(obj);
            }
            catch (ConcurrencyException ex)
            {
                TempData["ConcurrencyError"] = ex.Message;
                EventDTO obj = _mapper.Map<EventDTO>(_repository.GetEventById(id)); //????????????????????????????????
                return View(obj);
            }

        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(EventDTO obj)
        {
            try
            {
                bool result = _repository.Delete(obj.Id, _userManager.GetUserName(this.User));
            }
            catch (ConcurrencyException ex)
            {
                ModelState.AddModelError("", ex.Message);

                return View(obj);
            }


            return RedirectToAction(nameof(Index));
        }        
       
    }
}
