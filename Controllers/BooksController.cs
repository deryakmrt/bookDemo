using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using bookDemo.Data;
using bookDemo.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace bookDemo.Controllers
{
    [Route("api/books")] /*Bu ifade, bu controller'a nasıl erişileceğini belirten adresi tanımlar.
                     kısmı, sınıfın adı olan "Books" kelimesini alarak adresi otomatik olarak /api/Books yapar.*/
    [ApiController]
    public class BooksController : ControllerBase
    {   //====GET İSTEKLERİ====
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
        //====POST İSTEKLERİ====
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

        //====PUT İSTEKLERİ====
        [HttpPut("{id:int}")]
        public IActionResult UpdateOneBook([FromRoute(Name = "id")] int id,
            [FromBody] Book book)
        {
            //check book?
            var entity = ApplicationContext
                .Books
                .Find(b => b.Id.Equals(id));
            if (entity is null)
                return NotFound();//404

            //check id
            if(id!= book.Id)
                return BadRequest();//400
            ApplicationContext.Books.Remove(entity);
            book.Id = entity.Id;
            ApplicationContext.Books.Add(book);
                return Ok(book);

        }

        //====DELETE İSTEKLERİ====
        [HttpDelete]
        public IActionResult DeleteAllBooks()
        {
            ApplicationContext.Books.Clear();
                return NoContent();//204
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneBooks([FromRoute(Name = "id")] int id)
        {
            var entity = ApplicationContext
            .Books
            .Find(b=>b.Id.Equals(id));

            if (entity is null) return NotFound(new
            {
                statusCode = 404,
                message = $"Book with id:{id} could not found."
            });//404

            ApplicationContext.Books.Remove(entity);
                return NoContent();
        }

        //====PATCH İSTEKLERİ====
        [HttpPatch("{id:int}")]
        public IActionResult PartiallyUpdateOneBook([FromRoute(Name ="id")]int id, 
            [FromBody] JsonPatchDocument <Book> bookPatch)
        {
            //check entity
            var entity = ApplicationContext.Books.Find(b => b.Id.Equals(id));
            
            if(entity is null) return NotFound();//404

            bookPatch.ApplyTo(entity);
            return NoContent();//204
        }
    }
}
