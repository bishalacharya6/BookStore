﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Models;

namespace Web.DataAccess.Repository
{
    public interface ICatagoryRepository : IRepository<Catagory>
    {
        void Update(Catagory obj);

    }
}
