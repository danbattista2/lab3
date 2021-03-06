﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using LessonNineTwo.Models;
using System.Web.ModelBinding;

namespace LessonNineTwo
{
    public partial class course : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetDepartments();

                //get the course if editing
                if (!String.IsNullOrEmpty(Request.QueryString["CourseID"]))
                {
                    GetCourse();
                }
            }
        }

        protected void GetCourse()
        {
            Int32 CourseID = Convert.ToInt32(Request.QueryString["CourseID"]);
            //populate the existing course for editing
            using (DefaultConnection db = new DefaultConnection())
            {
                Course objC = (from c in db.Courses
                               where c.CourseID == CourseID
                               select c).FirstOrDefault();

                //populate the form
                txtTitle.Text = objC.Title;
                txtCredits.Text = objC.Credits.ToString();
                ddlDepartment.SelectedValue = objC.DepartmentID.ToString();

                //students table
                var objE = (from c in db.Enrollments
                            join dp in db.Students on c.StudentID equals dp.StudentID
                            join d in db.Courses on c.CourseID equals d.CourseID
                            where c.CourseID == CourseID
                            select new {c.EnrollmentID, c.StudentID, dp.LastName, dp.FirstMidName, dp.EnrollmentDate});

                grdStudents.DataSource = objE.ToList();
                grdStudents.DataBind();

                //clear dropdowns
                ddlStudent.ClearSelection();
                ddlCourse.ClearSelection();

                //fill studentdropdown
                var dd = from d in db.Students
                           //orderby d.FirstMidName
                           select d;

                ddlStudent.DataSource = dd.ToList();
                ddlStudent.DataBind();

                //add default options to the 2 dropdownsE
                ListItem newItem = new ListItem("-Select-", "0");
                ddlStudent.Items.Insert(0, newItem);
                ddlCourse.Items.Insert(0, newItem);

                //show the course panel
                pnlStudents.Visible = true;


            }
        }

        protected void GetDepartments()
        {

            Int32 DepartmentID = Convert.ToInt32(Request.QueryString["DepartmentID"]);

            using (DefaultConnection db = new DefaultConnection())
            {
                var deps = (from d in db.Departments
                            orderby d.Name
                            select d);

                ddlDepartment.DataSource = deps.ToList();
                ddlDepartment.DataBind();

            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //do insert or update
            using (DefaultConnection db = new DefaultConnection())
            {
                Course objC = new Course();

                if (!String.IsNullOrEmpty(Request.QueryString["CourseID"]))
                {
                    Int32 CourseID = Convert.ToInt32(Request.QueryString["CourseID"]);
                    objC = (from c in db.Courses
                            where c.CourseID == CourseID
                            select c).FirstOrDefault();
                }

                //populate the course from the input form
                objC.Title = txtTitle.Text;
                objC.Credits = Convert.ToInt32(txtCredits.Text);
                objC.DepartmentID = Convert.ToInt32(ddlDepartment.SelectedValue);

                if (String.IsNullOrEmpty(Request.QueryString["CourseID"]))
                {
                    //add
                    db.Courses.Add(objC);
                }

                //save and redirect
                db.SaveChanges();
                Response.Redirect("courses.aspx");
            }
        }

        //NOT WORKING, TRIED USING STUDENT ID AND ENROLLMENT ID, KEEPS THROWING ERROR WHEN 
        //PRESSING DELETE
        protected void grdStudents_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Int32 EnrollmentID = Convert.ToInt32(grdStudents.DataKeys[e.RowIndex].Values["EnrollmentID"]);

            using (DefaultConnection db = new DefaultConnection())
            {
                Enrollment objE = (from en in db.Enrollments
                                   where en.EnrollmentID == EnrollmentID
                                   select en).FirstOrDefault();

                //processs the deletetion
                db.Enrollments.Remove(objE);
                db.SaveChanges();

                //reopoulate the page
                GetCourse();


            }
        }

        protected void ddlStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (DefaultConnection db = new DefaultConnection())
            {
         
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {

        }
    }
}