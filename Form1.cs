using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Excel2Mysql
{
	public partial class Form1 : Form
	{
		private OleDbConnection c;

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			

		}

		private static OleDbConnection GetConnection(string filename, bool openIt)
		{
			//if your data has no header row, change HDR = NO
			var c = new OleDbConnection($"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{filename}';Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\" ");
			if (openIt)
				c.Open();
			return c;
		}

		private static DataSet GetExcelFileAsDataSet(OleDbConnection conn)
		{
			var sheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new[] { default, default, default, "TABLE" });
			var ds = new DataSet();
			foreach (DataRow r in sheets.Rows)
				ds.Tables.Add(GetExcelSheetAsDataTable(conn, r["TABLE_NAME"].ToString()));
			return ds;
		}

		private static DataTable GetExcelSheetAsDataTable(OleDbConnection conn, string sheetName)
		{
			using (var da = new OleDbDataAdapter($"select * from [{sheetName}]", conn))
			{
				var dt = new DataTable() { TableName = sheetName.TrimEnd('$') };
				da.Fill(dt);
				return dt;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			OpenFileDialog openfile = new OpenFileDialog();
			openfile.DefaultExt = ".xlsx";
			openfile.Filter = "Excel files|*.xls;*.xlsx";
			DialogResult result = openfile.ShowDialog();
			if (result == DialogResult.OK)
			{
				//All sheets
				DataSet ds;
				string fname=openfile.FileName;
				using (c = GetConnection(fname, true))
					ds = GetExcelFileAsDataSet(c);

				//only one sheet
				DataTable dt;
				using (c = GetConnection(fname, true))
					dt = GetExcelSheetAsDataTable(c, "hospital_registration$");
				dataGridView1.DataSource = dt;
			}
			else if (result == DialogResult.Cancel)
			{
				MessageBox.Show("Aborted!!");
			}
			
		}

	}
}
