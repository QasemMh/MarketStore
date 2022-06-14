using MarketStore.ViewModels;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MarketStore.constants
{
	public static class EmailService
	{


		private static string GetHtmlBody(OrderViewModel data, string contentRootPath)
		{
			string htmlTemplateFile = System.IO.File
				.ReadAllText(System.IO.Path.Combine(contentRootPath +
				"/constants/CustomerInvoice.html"));

			StringBuilder htmlTemplate = new StringBuilder();
			StringBuilder orderDetails = new StringBuilder();
			htmlTemplate.Append(htmlTemplateFile);

			htmlTemplate.Replace("#logo", "Market Store");
			htmlTemplate.Replace("#orderId", data.Order.Id + "");
			htmlTemplate.Replace("#orderDate", data.Order.OrderDate.Value.ToString("dd/MM/yyyy HH:mm tt"));
			htmlTemplate.Replace("#username", $"{data.Order.Customer.FirstName} {data.Order.Customer.LastName}");

			foreach (var item in data.OrderDetails)
			{
				orderDetails.Append(
					@$"<tr>
					  <td width ='80%' class='purchase_item'>
						<span class='f-fallback'>
							{item.Product.Name} x{item.Quantity}
						</span>
					  </td>
  
					  <td class='align-right' width='20%'>
						<span class='f-fallback'>
							{item.Product.Price * item.Quantity}
						</span>
					   </td>
					</tr>"
					);
			}
			htmlTemplate.Replace("#orderDetails", orderDetails.ToString());
			htmlTemplate.Replace("#total", data.OrderDetails
				.Select(o => o.Product.Price * o.Quantity).Sum().ToString());

			return htmlTemplate.ToString();
		}

		public static async Task<bool> SendAsync(OrderViewModel data, string contentRootPath)
		{
			try
			{
				string body = GetHtmlBody(data, contentRootPath);

				using SmtpClient mySmtpClient = new SmtpClient("smtp.gmail.com", 587);
				mySmtpClient.EnableSsl = true;

				// set smtp-client with basicAuthentication
				mySmtpClient.UseDefaultCredentials = false;
				NetworkCredential basicAuthenticationInfo = new
				  NetworkCredential("qm84028@gmail.com", "symygvvpsvfabsbo");
				mySmtpClient.Credentials = basicAuthenticationInfo;

				// add from,to mailaddresses
				MailAddress from = new MailAddress("qm84028@gmail.com", "Market Store Admin");
				MailAddress to = new MailAddress(data.Order.Customer.User.Email, "Customer:" + data.Order.Customer.FirstName);
				using MailMessage myMail = new MailMessage(from, to);



				// set subject and encoding
				myMail.Subject = "Invoice - MarketStore";
				myMail.SubjectEncoding = Encoding.UTF8;

				// set body-message and encoding
				myMail.Body = body;
				myMail.BodyEncoding = Encoding.UTF8;
				// text or html
				myMail.IsBodyHtml = true;

				await mySmtpClient.SendMailAsync(myMail);
				return true;
			}

			catch (SmtpException ex)
			{
				return false;
			}
			catch (Exception ex)
			{
				return false;
			}


		}
	}
}
