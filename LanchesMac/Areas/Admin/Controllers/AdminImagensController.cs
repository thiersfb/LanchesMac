using LanchesMac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
//using System.Threading.Tasks;

namespace LanchesMac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminImagensController : Controller
    {
        private readonly ConfigurationImagens _myConfig;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AdminImagensController(IWebHostEnvironment hostingEnvironment, IOptions<ConfigurationImagens> myConfiguration)
        {
            _hostingEnvironment = hostingEnvironment;
            _myConfig = myConfiguration.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UploadFiles(List<IFormFile> files) //"files" precisa ser este nome pois é o mesmo do input da view index.cshtml
        {
            if (files == null || files.Count == 0 )
            {
                ViewData["Erro"] = "Error: Arquivo(s) não selecionado(s)";
                return View(ViewData);
            }

            if(files.Count > 10)
            {
                ViewData["Erro"] = "Error: Quantidade de arquivos excedeu o limite";
                return View(ViewData);
            }

            long size = files.Sum(f => f.Length);

            var filePathsName = new List<string>();

            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, _myConfig.NomePastaImagensProdutos);

            foreach (var formFile in files)
            {
                if(formFile.FileName.Contains(".jpg") || formFile.FileName.Contains(".gif") || formFile.FileName.Contains(".png"))
                {
                    var fileNameWithPath = string.Concat(filePath, "\\", formFile.FileName);

                    filePathsName.Add(fileNameWithPath);

                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                } else
                {
                    ViewData["Erro"] = "Error: Extensões não permitidas";
                    return View(ViewData);
                }
            }
            ViewData["Resultado"] = $"{files.Count} arquivos foram enviados ao servidor, " + 
                                    $"com tamanho total de : {size} bytes";
            ViewBag.Arquivos = filePathsName;
            return View(ViewData);
        }

        public IActionResult GetImagens()
        {
            FileManagerModel fileManagerModel = new FileManagerModel();
            
            var userImagesPath = Path.Combine(_hostingEnvironment.WebRootPath, _myConfig.NomePastaImagensProdutos);

            DirectoryInfo dir = new DirectoryInfo(userImagesPath);

            FileInfo[] files = dir.GetFiles();

            fileManagerModel.PathImagensProduto = _myConfig.NomePastaImagensProdutos;

            if(files.Length ==0)
            {
                ViewData["Erro"] = $"Nenhum arquivo encontrado na pasta {userImagesPath}";
                return View(ViewData);
            }
            fileManagerModel.Files = files;

            return View(fileManagerModel);
        }

        public IActionResult DeleteFile(string fname)
        {
            var _imagemDeleta = Path.Combine(_hostingEnvironment.WebRootPath, _myConfig.NomePastaImagensProdutos + "\\", fname);

            if((System.IO.File.Exists(_imagemDeleta)))
            {
                System.IO.File.Delete(_imagemDeleta);
                ViewData["Deletado"] = $"Arquivo(s) {_imagemDeleta} excluído com sucesso";
            }

            return View("index");
        }
    }
}
