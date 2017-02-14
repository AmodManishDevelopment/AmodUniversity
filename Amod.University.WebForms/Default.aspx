<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Amod.University.WebForms._Default" ClientIDMode="Static"%>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper">
        </div>
    </section>
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

    <div>
        <asp:Label ID="Label1" runat="server" Text="First Name"></asp:Label>
        <asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox>
        <br />
        <asp:Label ID="Label2" runat="server" Text="Last Name"></asp:Label>
        <asp:TextBox ID="txtLastName" runat="server"></asp:TextBox>
        <br />
        <asp:Label ID="Label3" runat="server" Text="Enrollment Notes"></asp:Label>
        <asp:TextBox ID="txtEnrollmentNotes" runat="server"></asp:TextBox>
        <br />
        <asp:Button ID="btnCreateStudent" runat="server" Text="Create Student" />
    </div>

    <br />

    <div>
    <table id="tblStudents" class="display" width="100%" cellspacing="0">
        <thead>
            <tr>
                <th>Student ID</th>
                <th>First Name</th>
                <th>Last Name</th>
            </tr>
        </thead>
        <tfoot>
            <tr>
                <th>Student ID</th>
                <th>First Name</th>
                <th>Last Name</th>
            </tr>
        </tfoot>
    </table>
        </div>

    <script type="text/javascript">

        $(document).ready(function () {
            GetStudents();

            $("#btnCreateStudent").click(function () {
                //CreateStudent();

                var request = new Object();
                request.student = new Object();
                request.student.FirstName = $("#txtFirstName").val();
                request.student.LastName = $("#txtLastName").val();
                request.EnrollmentNotes = $("#txtEnrollmentNotes").val();

                //var request = { request: createStudentRequest };

                $.ajax("http://localhost:33472/createstudent", {
                    data: request,
                    type: "POST",
                    dataType: "json",
                    success: function (result, txt, o) {
                        if (txt != 'success') {
                            alert(result.message);
                        }
                        else {
                            GetStudents();
                        }
                    },
                    error: function (result, txt, o) {
                        alert(result.message);
                    }
                });

                return false;
            });
        });

        function GetStudents() {
            $.ajax("http://localhost:33472/getstudents", {
                data: '',
                type: "GET",
                dataType: "json",
                success: function (result, txt, o) {
                    if (txt != 'success') {
                        alert(result.message);
                    }
                    else {
                        if (result.students.length > 0) {
                            $('#tblStudents').DataTable({
                                "responsive": true,
                                "destroy": true,
                                "paging": true,
                                "ordering": true,
                                "info": true,
                                "searching": true,
                                "data": result.students,
                                "columns": [
                                    { "data": "studentID" },
                                    { "data": "firstName" },
                                    { "data": "lastName" }
                                ]
                            });
                        }
                    }
                },
                error: function (result, txt, o) {
                    alert(result.message);
                }
            });
        }

        //function CreateStudent() {
        //    var createStudentRequest = new Object();
        //    createStudentRequest.student = new Object();
        //    createStudentRequest.student.FirstName = $("#txtFirstName").val();
        //    createStudentRequest.student.LastName = $("#txtLastName").val();
        //    createStudentRequest.EnrollmentNotes = $("#txtEnrollmentNotes").val();

        //    var s = JSON.stringify(createStudentRequest);

        //    $.ajax("http://localhost:33472/createstudent", {
        //        data: JSON.stringify(createStudentRequest),
        //        type: "POST",
        //        dataType: "json",
        //        success: function (result, txt, o) {
        //            if (txt != 'success') {
        //                alert(result.message);
        //            }
        //            else {
        //                GetStudents();
        //            }
        //        },
        //        error: function (result, txt, o) {
        //            alert(result.message);
        //        }
        //    });
        //}
    </script>

</asp:Content>


