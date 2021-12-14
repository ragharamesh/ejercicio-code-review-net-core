using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Calculadora.Models;
using Calculadora.Notations;
using Calculadora.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Calculadora.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ApiController : ControllerBase
    {
        private readonly IEnumerable<ICalculatorService> _calculatorServices;
        private static List<HistoricoBaseViewModel> historicoList = new List<HistoricoBaseViewModel>(0);

        private readonly ILogger<ApiController> _logger;

        public ApiController(
            ILogger<ApiController> logger,
            IEnumerable<ICalculatorService> calculatorServices
        ) {
            _calculatorServices = calculatorServices;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<HistoricoBaseViewModel> Get() => historicoList;

        [HttpPost]
        public ActionResult Calcular([FromBody] RequestViewModel request)
        {
            var service = _calculatorServices.Where(c => c.CodigoOperacao == request.Operacao) ?? throw new ArgumentException("Service not found");

            var response = service.FirstOrDefault().Execute(request);

            var historico = new HistoricoBaseViewModel(request, response);

            _logger.LogInformation("history", historico);

            historicoList.Add(historico);

            return Ok(response);
        }

        [HttpGet]
        public FileResult Download()
        {
            var fileName = $"Historico_{DateTime.Now.ToString("yyyyMMDD")}.csv";

            return new FileContentResult(GetFile(), "text/csv")
            {
                FileDownloadName = fileName
            };
        }

        private static byte[] GetFile()
        {
            var sb = new StringBuilder();

            var campos = GetCampos<HistoricoViewModel>();

            foreach (var itemCampo in campos)
            {
                var valor = GetLayout(itemCampo);

                sb.Append($"{valor.Title};");
            }

            sb.Append(Environment.NewLine);

            foreach (var item in historicoList)
            {
                sb.Append($"{item.Date};{item.Numero1};{item.Operacao};{item.Numero2};{item.Resultado};");
                sb.Append(Environment.NewLine);
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private static IEnumerable<PropertyInfo> GetCampos<T>()
        {
            return typeof(T).GetProperties().Where(x => x.CustomAttributes.Any()).ToList();
        }

        private static TitleToExport GetLayout(PropertyInfo field)
        {
            return ((TitleToExport)field.GetCustomAttributes(typeof(TitleToExport), true).First());
        }


     public static void fibonacci()  
      {  
         int n1=0,n2=1,n3,i,number;    
         Console.Write("Enter the number of elements: ");    
         number = int.Parse(Console.ReadLine());  
         Console.Write(n1+" "+n2+" "); //printing 0 and 1    
         for(i=2;i<number;++i) //loop starts from 2 because 0 and 1 are already printed    
         {    
          n3=n1+n2;    
          Console.Write(n3+" ");    
          n1=n2;    
          n2=n3;    
         }    
      }  
    }
}
