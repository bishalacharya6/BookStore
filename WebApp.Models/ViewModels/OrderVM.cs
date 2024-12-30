using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Models.ViewModels
{
	public class OrderVM
	{
		public IEnumerable<OrderDetail> OrderDetails {  get; set; }
		public OrderHeader OrderHeader { get; set; }

	}
}
