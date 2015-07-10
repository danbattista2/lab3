using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//reference models
using LessonNineTwo.Models;
using System.Web.ModelBinding;

namespace LessonNineTwo
{
    public partial class department : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if the page isn't posted back, check the url for an id to see know add or edit
            if ((!IsPostBack)&& (Request.QueryString.Keys.Count > 0))
            {
                    GetDepartment();
            }
        }

        protected void GetDepartment()
        {
            //get id from url parameter and store in a variable
            Int32 DepartmentID = Convert.ToInt32(Request.QueryString["DepartmentID"]);
            //connect
            using (DefaultConnection db = new DefaultConnection())
            {
                //populate a department instance with the DeparmtnetID from the URL parameter
                Department d = (from dep in db.Departments
                         where dep.DepartmentID == DepartmentID
                         select dep).FirstOrDefault();

                //populate the form from our department object
               txtName.Text = d.Name;
               txtBudget.Text = d.Budget.ToString();

               //courses table
               var objE = (from c in db.Courses
                           join dp in db.Departments on c.DepartmentID equals dp.DepartmentID
                           where c.DepartmentID == DepartmentID
                           select new { c.CourseID, c.Title, c.Credits});

               grdCourses.DataSource = objE.ToList();
               grdCourses.DataBind();

                //show the course panel
                pnlCourses.Visible = true;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //connect
            using (DefaultConnection conn = new DefaultConnection())
            {
                //instantiate a new deparment object in memory
                Department d = new Department();

                //decide if updating or adding, then save
                if (Request.QueryString.Count > 0)
                {
                    Int32 DepartmentID = Convert.ToInt32(Request.QueryString["DepartmentID"]);

                    d = (from dep in conn.Departments
                         where dep.DepartmentID == DepartmentID
                         select dep).FirstOrDefault();
                }

                //fill the properties of our object from the form inputs
                d.Name = txtName.Text;
                d.Budget = Convert.ToDecimal(txtBudget.Text);

                if (Request.QueryString.Count == 0)
                {
                    conn.Departments.Add(d);
                }
                conn.SaveChanges();

                //redirect to updated departments page
                Response.Redirect("departments.aspx");
            }
        }

        protected void grdCourses_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Int32 CourseID = Convert.ToInt32(grdCourses.DataKeys[e.RowIndex].Values["CourseID"]);

            using (DefaultConnection db = new DefaultConnection())
            {
                Course objE = (from en in db.Courses
                                   where en.CourseID == CourseID
                                   select en).FirstOrDefault();

                //processs the deletetion
                db.Courses.Remove(objE);
                db.SaveChanges();

                //reopoulate the page
                GetDepartment();
            }
        }
    }
}