<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Amod.University.WebForms._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper">
        </div>
    </section>
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <table id="example" class="display" width="100%" cellspacing="0">
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

    <script type="text/javascript">

        $(document).ready(function () {
            //$('#example').DataTable({
            //    "responsive": true,
            //    "paging": true,
            //    "ordering": true,
            //    "info": true,
            //    "searching": true,
            //    "ajax": {
            //        "url": "http://localhost:33472/getstudents",
            //        "dataSrc": "students"
            //    },
            //    "columns": [
            //        { "data": "studentID" },
            //        { "data": "firstName" },
            //        { "data": "lastName" }
            //    ]
            //});

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
                            $('#example').DataTable({
                                "responsive": true,
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
        });


    </script>

</asp:Content>


