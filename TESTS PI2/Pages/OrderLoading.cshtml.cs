using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using OxyPlot;
using OxyPlot.Blazor;

namespace TESTS_PI2.Pages
{
    public class OrderLoadingModel : PageModel
    {
        private IWebHostEnvironment _environment;

        public PlotModel plotmodel { get; set; }
        public string orderFilePath { get; set; }
        public string Msg { get; set; }
        public string orderLoadMsg { get; set; }
        public bool RenderLoading { get; set; }
        public bool RenderAnalysis { get; set; }
        public List<Order> ordersEntry { get; set; }
        public List<Order> ordersEntryFilteredSymbol { get; set; }
        public string ordersEntryFilteredSymbolMsg { get; set; }
        public List<Order> ordersEntryFilteredDate { get; set; }
        public string ordersEntryFilteredDateMsg { get; set; }
        public List<Order> ordersEntryFilteredFrame { get; set; }
        public string ordersEntryFilteredFrameMsg { get; set; }
        public List<Order> ordersEntryFilteredType { get; set; }
        public string ordersEntryFilteredTypeMsg { get; set; }
        public List<Order> ordersEntryFilteredVolume { get; set; }
        public string ordersEntryFilteredVolumeMsg { get; set; }
        public List<Order> ordersEntriesFilteredAll { get; set; }
        public string ordersEntriesFilteredAllMsg { get; set; }
        public OrderLoadingModel(IWebHostEnvironment environment)
        {
            _environment = environment;
            RenderLoading = false;
        }
        
        public void OnPostUpload(List<IFormFile> uploadedFiles)
		{
            string pathwww = this._environment.WebRootPath;
            string orderPath = this._environment.ContentRootPath;
            string path = Path.Combine(pathwww, "Orders");
			if (!Directory.Exists(path)){ Directory.CreateDirectory(path); }
            List<string> ordersUploaded = new List<string>();
            foreach(IFormFile uploadedFile in uploadedFiles)
			{
                string fileName = Path.GetFileName(uploadedFile.FileName);
                string filePath = Path.Combine(path, fileName);
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
				{
                    uploadedFile.CopyTo(fileStream);
                    ordersUploaded.Add(fileName);
                    this.Msg += String.Format("<b>{0}</b> uploaded to server <br/>", fileName);
                }
            }
            this.RenderLoading = true;
        }
        
        public void OnPostOrderRead()
        {
            try
            {
                ordersEntry = Order.ReadingOrders(orderFilePath);
                orderLoadMsg = "<b>File successfully loaded in memory</b>";
                RenderAnalysis = true;
            }
            catch (Exception)
            {
                orderLoadMsg = "Error while attempting to read the file";
                throw;
            }
        }

        public void OnPostFilterSymbol(string symbol)
        {
            symbol = symbol.ToLower();
            try
            {
                ordersEntryFilteredSymbol = Order.FilterOrdersBySymbol(ordersEntry, symbol);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void OnPostFilterDate(string startDate, string endDate)
        {
            
            try
            {
                DateTime formatedStartDate = DateTime.ParseExact(startDate, "yyyy-MM-dd HH:mm:ss.fffZ", CultureInfo.InvariantCulture);
                DateTime formatedEndDate = DateTime.ParseExact(endDate, "yyyy-MM-dd HH:mm:ss.fffZ", CultureInfo.InvariantCulture);
                ordersEntryFilteredDate = Order.FilterOrdersByDate(ordersEntry, formatedStartDate, formatedEndDate);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void OnPostFilterFrame(string startHour, string endHour)
        {
            try
            {
                int startHourInt = int.Parse(startHour);
                int endHourInt = int.Parse(endHour);
                ordersEntryFilteredFrame = Order.FilterOrdersByHour(ordersEntry, startHourInt, endHourInt);
                ordersEntryFilteredFrameMsg = "Filter by frame successfully applied";
            }
            catch (Exception)
            {
                ordersEntryFilteredFrameMsg = "Error while attempting to filter by timeframe";
                throw;
            }
        }

        public void OnPostFilterFill(string minFill)
        {
            try
            {
                int fillInt = int.Parse(minFill);
                if (fillInt>100 || fillInt<0)
                {
                    ordersEntryFilteredTypeMsg = "Fill% must be between 0 and 100";
                }
                else
                {
                    ordersEntryFilteredType = Order.FilterOrdersByExecutedVolumeProportion(ordersEntry, fillInt);
                    ordersEntryFilteredTypeMsg = "Filter by fill successfully applied";
                }
                
            }
            catch (Exception)
            {
                ordersEntryFilteredTypeMsg = "Error while attempting to filter by fill";
                throw;
            }
        }
        public void OnPostFilterType(string type)
        {
            try
            {
                bool buy = false;
                if (type=="true")
                {
                    buy = true;                    
                }
                ordersEntryFilteredType = Order.FilterOrdersByType(ordersEntry, buy);
                ordersEntryFilteredTypeMsg = "Filter by type successfully applied";
            }
            catch (Exception)
            {
                ordersEntryFilteredTypeMsg = "Error while attempting to filter by type";
                throw;
            }
        }

        public void OnPostFilterAll(string symbol, string startDate, string endDate, string startHour, string endHour, string minFill, string type)
        {
            try
            {
                DateTime formatedStartDate = DateTime.ParseExact(startDate, "yyyy-MM-dd HH:mm:ss.fffZ", CultureInfo.InvariantCulture);
                DateTime formatedEndDate = DateTime.ParseExact(endDate, "yyyy-MM-dd HH:mm:ss.fffZ", CultureInfo.InvariantCulture);
                int startHourInt = int.Parse(startHour);
                int endHourInt = int.Parse(endHour);
                double fillProportion = double.Parse(minFill);
                bool buy = false;
                if (type == "true")
                {
                    buy = true;
                }
                ordersEntriesFilteredAll = Order.FilterAny(ordersEntry, buy, fillProportion, symbol, formatedStartDate, formatedEndDate, startHourInt, endHourInt);
                ordersEntriesFilteredAllMsg = "Filter by all successfully applied";
            }
            catch (Exception)
            {
                ordersEntriesFilteredAllMsg = "Error while attempting to filter by all";
                throw;
            }
        }

        public void OnPostTEST()
        {
            PlotModel tempModel = new PlotModel { Title = "TEST" };
            
        }


        public void OnGet()
        {
            
        }
    }
}
