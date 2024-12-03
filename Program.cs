using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace EntradaNotasXML
{
    public class NotaFiscal
    {
        public string Numero { get; set; }
        public string Emissor { get; set; }
        public string Destinatario { get; set; }
        public decimal ValorTotal { get; set; }
        public List<ItemNota> Itens { get; set; } = new List<ItemNota>();

        public override string ToString()
        {
            return $"Número: {Numero}\nEmissor: {Emissor}\nDestinatário: {Destinatario}\nValor Total: {ValorTotal:C}\nItens:\n{string.Join("\n", Itens)}";
        }
    }

    public class ItemNota
    {
        public string Descricao { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal => Quantidade * ValorUnitario;

        public override string ToString()
        {
            return $"- {Descricao} | Qtde: {Quantidade} | Valor Unitário: {ValorUnitario:C} | Total: {ValorTotal:C}";
        }
    }

    public class ProcessadorXML
    {
        public NotaFiscal LerNotaFiscal(string caminhoArquivo)
        {
            if (!File.Exists(caminhoArquivo))
            {
                throw new FileNotFoundException("O arquivo XML não foi encontrado.", caminhoArquivo);
            }

            var documento = XDocument.Load(caminhoArquivo);
            var nfe = new NotaFiscal();

            // Lendo dados principais da nota
            nfe.Numero = documento.Root.Element("infNFe").Element("ide").Element("nNF")?.Value;
            nfe.Emissor = documento.Root.Element("infNFe").Element("emit").Element("xNome")?.Value;
            nfe.Destinatario = documento.Root.Element("infNFe").Element("dest").Element("xNome")?.Value;
            nfe.ValorTotal = decimal.Parse(documento.Root.Element("infNFe").Element("total").Element("ICMSTot").Element("vNF")?.Value);

            // Lendo itens da nota
            foreach (var det in documento.Root.Element("infNFe").Elements("det"))
            {
                var item = new ItemNota
                {
                    Descricao = det.Element("prod").Element("xProd")?.Value,
                    Quantidade = int.Parse(det.Element("prod").Element("qCom")?.Value),
                    ValorUnitario = decimal.Parse(det.Element("prod").Element("vUnCom")?.Value)
                };

                nfe.Itens.Add(item);
            }

            return nfe;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var processador = new ProcessadorXML();

            Console.WriteLine("=== Entrada de Notas Fiscais por XML ===");
            Console.Write("Informe o caminho do arquivo XML: ");
            string caminhoArquivo = Console.ReadLine();

            try
            {
                var notaFiscal = processador.LerNotaFiscal(caminhoArquivo);
                Console.WriteLine("\nNota Fiscal processada com sucesso:");
                Console.WriteLine(notaFiscal);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar o XML: {ex.Message}");
            }
        }
    }
}
