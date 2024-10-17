using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.Services;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IMailService _mailService;
        private readonly ISeedService _seedService;
        private readonly DecorVistaDbContext _context;

        public TestController(IFileService fileService,IMailService mailService, ISeedService seedService, DecorVistaDbContext decorVistaDbContext)
        {
            _fileService = fileService;
            _mailService = mailService;
            _seedService = seedService;
            _context = decorVistaDbContext;
        }

        [HttpPost("testUploadFile")]
        public async Task<IActionResult> TestUploadImage(IFormFile file)
        {
            var result = await _fileService.UploadImageAsync(file);
            return Ok(result);
        }

        [HttpPost("TestSendMail")]
        public async  Task<IActionResult> TestSendMail(string emailReceiver, string subject, string message)
        {
            _mailService.SendMailAsync(emailReceiver, subject, message);
            return Ok("");
        }


        [HttpGet("TestSeedProduct")]
        public IActionResult TestSeedProduct()
        {
            _seedService.SeedProduct();
            return Ok("");
        }

        [HttpGet("TestGetProduct")]
        public IActionResult TestGetProduct()
        {
            var products = _context.Products.Include(p => p.functionality).ToList();
            return Ok(products);
        }


        [HttpPost("seed_user")]
        public async Task<IActionResult> SeedUser()
        {
            for(var i = 0; i < 100; i++){
                var newUser = new User
                {
                    email = Faker.Name.First().ToLower() + Faker.Name.Suffix().ToLower() + "@gmail.com",
                    password = "$2a$12$9ua8xqYXL3hK2uqpUfJi.eGOUN9yq6guZEyf7CZ744cDKpvFpu1T6"
                };

                var newUserDetail = new UserDetails
                {
                    role = "customer",
                    address = Faker.Address.StreetAddress(),
                    contact_number = Faker.Phone.Number().Replace("-", ""),
                    first_name = Faker.Name.First(),
                    last_name = Faker.Name.Last(),
                    User = newUser,
                    avatar = "2fd7c649-6851-4b96-8752-ad239d4e1c19.jpg"
                };

                _context.Users.Add(newUser);
                _context.UserDetails.Add(newUserDetail);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [NonAction]
        private DateTime GetRandomDate()
        {
            DateTime startDate = new DateTime(2024, 4, 1);
            DateTime endDate = new DateTime(2024, 7, 11);

            Random random = new Random();
            int range = (endDate - startDate).Days;

            return startDate.AddDays(random.Next(range + 1));
        }

        [HttpPost("seed_order")]
        public async Task<IActionResult> SeedOrder()
        {
            var random = new Random();

            var users = await _context.Users.Include(u => u.userdetails).Where(u => u.id >= 11).ToListAsync();

            foreach(var user in users)
            {
                var totalOrder = random.Next(1,10);

                for(var i = 0; i < totalOrder; i++)
                {
                    float total = 10;
                    var newOrder = new Order()
                    {
                        user = user,
                        address = Faker.Address.StreetAddress(),
                        phone = user.userdetails.contact_number,
                        fullname = user.userdetails.first_name + " " + user.userdetails.last_name,
                        created_date = GetRandomDate(),
                        updated_date = GetRandomDate().AddDays(1),
                        status = "completed"
                    };

                    for(var j = 0; j < 3; j++)
                    {
                        var variantId = random.Next(1,136);
                        var variant = await _context.Variants.Include(v => v.product).SingleOrDefaultAsync(v => v.id == variantId);

                        total += variant.price;
                        
                        var newOrderDetail = new OrderDetails()
                        {
                            order = newOrder,
                            variant = variant,
                            quanity =1,
                            review_status = true
                        };

                        _context.OrderDetails.Add(newOrderDetail);

                        var newReview = new Review()
                        {
                            comment = GetRandomComment(),
                            user = user,
                            product_id = variant.productid,
                            score = random.Next(3, 6),
                        };

                        _context.Reviews.Add(newReview);

                        newOrder.total = total;
                        _context.Orders.Add(newOrder);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("seed_subcribe")]
        public async Task<IActionResult> SeedSubcribe()
        {
            var random = new Random();

            var users = await _context.Users.Include(u => u.userdetails).Where(u => u.id >= 11).ToListAsync();

            foreach (var user in users)
            {
                var galleryId1 = 0;
                var galleryId2 = 0;
                var galleryId3 = 0;
                var galleryId4 = 0;
                var galleryId5 = 0;
                var galleryId6 = 0;

           
                galleryId1 = random.Next(4, 8);
                galleryId2 = random.Next(8, 12);
                galleryId3= random.Next(12, 16);
                galleryId4 = random.Next(16, 19);
                galleryId5 = random.Next(19, 23);
                galleryId6 = random.Next(23, 25);
             

                
                var newSubscribe1 = new Subcribe()
                {
                    user = user,
                    gallery_id = galleryId1
                };

                var newSubscribe2 = new Subcribe()
                {
                    user = user,
                    gallery_id = galleryId2
                };
                var newSubscribe3 = new Subcribe()
                {
                    user = user,
                    gallery_id = galleryId3
                };
                var newSubscribe4 = new Subcribe()
                {
                    user = user,
                    gallery_id = galleryId4
                };
                var newSubscribe5 = new Subcribe()
                {
                    user = user,
                    gallery_id = galleryId5
                };
                var newSubscribe6 = new Subcribe()
                {
                    user = user,
                    gallery_id = galleryId6
                };

                _context.Subcribes.Add(newSubscribe1);
                _context.Subcribes.Add(newSubscribe2);
                _context.Subcribes.Add(newSubscribe3);
                _context.Subcribes.Add(newSubscribe4);
                _context.Subcribes.Add(newSubscribe5);
                _context.Subcribes.Add(newSubscribe6);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("seed_consultation")]
        public async Task<IActionResult> SeedConsultation()
        {
            var random = new Random();

            var users = await _context.Users.Include(u => u.userdetails).Where(u => u.id >= 11).ToListAsync();

            foreach (var user in users)
            {
                for(var i = 0; i < 3; i++)
                {
                    var designerId = random.Next(1, 8);
                    var date = GetRandomDate();

                    var newConsultation = new Consultation()
                    {
                        designer_id = designerId,
                        time = "12:00",
                        user = user,
                        scheduled_datetime = date,
                        address = Faker.Address.StreetAddress(),
                        status = "finished",
                        notes = "I need your help"
                    };

                    _context.Consultations.Add(newConsultation);
                    var newReview = new Review()
                    {
                        user = user,
                        score = random.Next(3, 6),
                        create_at = date.AddDays(1),
                        comment = GetRandomCommentConsultation(),
                        Consultation = newConsultation
                    };

                    _context.Reviews.Add(newReview);
                  
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [NonAction]
        public static string GetRandomComment()
        {

            List<string> comments = new List<string>
        {
            "Great design, love the modern look!",
            "The quality of this product is amazing.",
            "I'm impressed by the attention to detail.",
            "This design truly stands out.",
            "Beautifully crafted, highly recommended!",
            "The product exceeded my expectations.",
            "I love the minimalistic approach.",
            "Stunning design with excellent functionality.",
            "Very unique and creative piece!",
            "The craftsmanship is top-notch.",
            "A perfect blend of style and utility.",
            "This product is a conversation starter.",
            "High-quality materials used.",
            "The design feels luxurious.",
            "Perfect for my home decor needs.",
            "Amazing design, very happy with this purchase.",
            "This piece has a timeless appeal.",
            "Elegant and stylish, great work!",
            "The product is as beautiful as it looks in pictures.",
            "Impressive design and build quality.",
            "Adds a touch of class to any space.",
            "The details are simply stunning.",
            "Crafted with precision and care.",
            "The designer has done a great job.",
            "It's both functional and stylish.",
            "Love the sleek design and finish.",
            "It’s the perfect addition to my collection.",
            "The design is simple yet elegant.",
            "Very durable and well-made.",
            "I get compliments all the time on this piece.",
            "This is a work of art!",
            "Very innovative design.",
            "It stands out in any room.",
            "Superb craftsmanship!",
            "Worth every penny for the design.",
            "A wonderful piece of design.",
            "A perfect gift for design enthusiasts.",
            "Incredible design and very practical.",
            "The product is designed to perfection.",
            "It blends well with my interior.",
            "A true designer’s masterpiece.",
            "It’s both beautiful and functional.",
            "Perfectly matches my taste in design.",
            "This design feels very high-end.",
            "I can’t imagine my space without it.",
            "Unique design that’s hard to find.",
            "It’s stylish yet very practical.",
            "Great value for such a well-designed piece.",
            "A must-have for design lovers.",
            "It’s the perfect statement piece.",
            "Absolutely love this designer product."
        };
            Random random = new Random();
            int index = random.Next(comments.Count);
            return comments[index];
        }

        [NonAction]
        public static string GetRandomCommentConsultation()
        {

            List<string> comments = new List<string>
        {
            "The consultation was incredibly insightful.",
            "I appreciate the designer's attention to my needs.",
            "Great advice on how to make the most of my space.",
            "The designer was very professional and knowledgeable.",
            "I'm thrilled with the design ideas presented to me.",
            "The consultation exceeded my expectations.",
            "I received some fantastic suggestions for my home decor.",
            "Very helpful in understanding my design style.",
            "The designer's expertise really showed during the session.",
            "Highly recommend the consultation service.",
            "I felt listened to and understood by the designer.",
            "The suggestions were practical and stylish.",
            "I got exactly what I was looking for from this consultation.",
            "The designer provided a fresh perspective on my space.",
            "The consultation was worth every penny.",
            "I now have a clear vision for my home thanks to the designer.",
            "Very thorough and detailed consultation.",
            "The designer gave me some unique ideas for my space.",
            "I appreciated the designer's honesty and creativity.",
            "The consultation was both informative and inspiring.",
            "I feel much more confident in my design choices now.",
            "The designer had a great sense of style.",
            "I loved the personalized approach during the consultation.",
            "The designer helped me see the potential in my space.",
            "Very pleased with the outcome of the consultation.",
            "The consultation was tailored to my specific needs.",
            "The designer's suggestions transformed my living room.",
            "The session was very productive and enjoyable.",
            "I highly value the advice given during the consultation.",
            "The designer's input made a big difference.",
            "Excellent service and wonderful ideas.",
            "I would definitely use this consultation service again.",
            "The designer had a great eye for detail.",
            "The consultation provided a lot of value for me.",
            "The designer's recommendations were spot on.",
            "I received clear and actionable advice during the session.",
            "The consultation helped me make the right decisions.",
            "It was a pleasure working with such a talented designer.",
            "The ideas were creative yet practical.",
            "I left the consultation feeling inspired and excited.",
            "The designer made the process so easy and enjoyable.",
            "The consultation helped me refine my vision.",
            "I appreciated the designer's patience and expertise.",
            "The suggestions fit my style perfectly.",
            "I got more out of the consultation than I expected.",
            "The designer gave me new ways to think about my space.",
            "Very satisfied with the consultation experience.",
            "The designer provided a fresh and modern perspective.",
            "The session helped me clarify my design goals.",
            "I feel more confident in my design direction now."
        };
            Random random = new Random();
            int index = random.Next(comments.Count);
            return comments[index];
        }



    } 
}
