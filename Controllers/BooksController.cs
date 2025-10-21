using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using bookDemo.Data;
using bookDemo.Models;

namespace bookDemo.Controllers
{
    [Route("api/books")] /*Bu ifade, bu controller'a nasıl erişileceğini belirten adresi tanımlar.
                     kısmı, sınıfın adı olan "Books" kelimesini alarak adresi otomatik olarak /api/Books yapar.*/
    [ApiController]
    public class BooksController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllBooks()
        {
            var books = ApplicationContext.Books;//ApplicationContext içerisindeki Books listesini alır
                                                 //ve bu listeyi Ok (HTTP 200 kodu - yani "başarılı") durumuyla birlikte cevap olarak gönderir.
            return Ok(books);
        }
        //2 farklı get isteğini ayırt eder
        //filtreleme yaptık
        [HttpGet("{id:int}")]
        public IActionResult GetOneBooks([FromRoute (Name = "id")] int id)//parametreler karışmasın diye
        {
            var book = ApplicationContext
                .Books
                .Where (b => b.Id.Equals (id))
                .SingleOrDefault();

            if (book is null)
                return NotFound(); //error 404
              
            return Ok(book);
        }

        [HttpPost]
        public IActionResult CreateOneBook([FromBody]Book book)
        {
            try
            {
                if (book is null)
                    return BadRequest(); //400
                ApplicationContext.Books.Add (book);
                return StatusCode(201, book);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

    }
}
