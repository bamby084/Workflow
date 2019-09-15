using System;
using System.Windows.Forms;
using System.IO;

namespace ScriptingApp
{
    class test
    {
        public class Input_0
        {
            public Input_0_Invoices Invoices { get; set; }
        }
        public class Input_0_Invoices
        {
            public Input_0_Invoice Invoice { get; set; }
        }
        public class Input_0_Invoice
        {
            public string ID { get; set; }
            public Input_0_LOBS LOBS { get; set; }
            public string TransactionDate { get; set; }
            public string Company { get; set; }
            public string add1 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZIP { get; set; }
            public string BilltoCompany { get; set; }
            public string Billtoadd1 { get; set; }
            public string BilltoCity { get; set; }
            public string BilltoState { get; set; }
            public string BilltoZIP { get; set; }
            public string ShiptoCompany { get; set; }
            public string Shiptoadd1 { get; set; }
            public string ShiptoCity { get; set; }
            public string ShiptoState { get; set; }
            public string ShiptoZIP { get; set; }
            public string SM { get; set; }
            public Input_0_items items { get; set; }
            public string GST { get; set; }
            public string Total { get; set; }
            public int SubTotal { get; set; }
        }
        public class Input_0_LOBS
        {
            public Input_0_LOB LOB { get; set; }
        }
        public class Input_0_LOB
        {
            public Input_0_Product Product { get; set; }
        }
        public class Input_0_Product
        {
            public string Type { get; set; }
        }
        public class Input_0_items
        {
            public Input_0_item item { get; set; }
        }
        public class Input_0_item
        {
            public string QTY { get; set; }
            public string ItemNumber { get; set; }
            public string Description { get; set; }
            public string UnitPrice { get; set; }
            public int Total { get; set; }
        }

        public class Output_0
        {
            public Output_0_Invoices Invoices { get; set; }
        }
        public class Output_0_Invoices
        {
            public Output_0_Invoice Invoice { get; set; }
        }
        public class Output_0_Invoice
        {
            public string ID { get; set; }
            public Output_0_LOBS LOBS { get; set; }
            public string TransactionDate { get; set; }
            public string Company { get; set; }
            public string add1 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZIP { get; set; }
            public string BilltoCompany { get; set; }
            public string Billtoadd1 { get; set; }
            public string BilltoCity { get; set; }
            public string BilltoState { get; set; }
            public string BilltoZIP { get; set; }
            public string ShiptoCompany { get; set; }
            public string Shiptoadd1 { get; set; }
            public string ShiptoCity { get; set; }
            public string ShiptoState { get; set; }
            public string ShiptoZIP { get; set; }
            public string SM { get; set; }
            public Output_0_items items { get; set; }
            public string GST { get; set; }
            public string Total { get; set; }
            public int SubTotal { get; set; }
        }
        public class Output_0_LOBS
        {
            public Output_0_LOB LOB { get; set; }
        }
        public class Output_0_LOB
        {
            public Output_0_Product Product { get; set; }
        }
        public class Output_0_Product
        {
            public string Type { get; set; }
        }
        public class Output_0_items
        {
            public Output_0_item item { get; set; }
        }
        public class Output_0_item
        {
            public string QTY { get; set; }
            public string ItemNumber { get; set; }
            public string Description { get; set; }
            public string UnitPrice { get; set; }
            public int Total { get; set; }
        }

        public class Transform
        {
            Input_0 Input_0 = new Input_0();
            Output_0 Output_0 = new Output_0();

            public void Log(object Message) { StreamWriter sw = null; try { sw = new StreamWriter(@"D:\ScriptingApp\ScriptingApp\bin\Debug\\Log.txt", true); sw.WriteLine(Message); sw.Flush(); sw.Close(); } catch { } }
            public void UpdateText()
            {
                Log("Test");

            }
        }
    }
}
