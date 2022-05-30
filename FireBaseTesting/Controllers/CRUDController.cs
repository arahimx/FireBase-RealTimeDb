using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireBaseTesting.Models;
using System.Web.Mvc;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FireBaseTesting.Controllers
{
    public class CRUDController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        { 
            AuthSecret = "M04MvSSfSd2h4INoyCJzTEohxD3MbiEsBR0Edvcu",
            BasePath = "https://causal-fort-313815-default-rtdb.firebaseio.com/"
        };
        IFirebaseClient client;
        // GET: Testing
        [Authorize]
        public ActionResult Index()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Students");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            if (data!=null)
            {
                var list = new List<Student>();
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<Student>(((JProperty)item).Value.ToString()));
                }

                return View(list);
            }
            ModelState.AddModelError(string.Empty, "Record not found");
            return View();

        }
        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        } 
        [HttpPost]
        public ActionResult Create(Student student)
        {
            try
            {
                AddStudentToFirebase(student);
                ModelState.AddModelError(string.Empty, "Added Successfully");
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Details(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Students/"+id);
            Student data = JsonConvert.DeserializeObject<Student>(response.Body);
            return View(data);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Students/" + id);
            Student data = JsonConvert.DeserializeObject<Student>(response.Body);
            return View(data);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(Student student)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                SetResponse setResponse = client.Set("Students/" + student.studentId, student);
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View();
        }






        private void AddStudentToFirebase(Student student)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = student;
            PushResponse response = client.Push("Students/", data);
            data.studentId = response.Result.name;
            SetResponse setResponse = client.Set("Students/"+data.studentId, data);

        }
    }

}