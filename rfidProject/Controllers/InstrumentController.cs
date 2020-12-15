

using System;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;
using System.Net.Http;
using MySql.Data.MySqlClient;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace project.Controllers{

 public class InstrumentController : ControllerBase{

        // Connects to database and execute an SQL statement for retrieving the data for Countries
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public DataSet executeSQL (string sqlStatement)
        {

            string connStr = "server=localhost; port=3306; user=root; password=LSWPMGy825mv1u; database=Medical_Surgeries";
            MySqlConnection conn = new MySqlConnection(connStr);
            MySqlDataAdapter sqlAdapter = new MySqlDataAdapter(sqlStatement,conn);
            DataSet myResultSet = new DataSet();
            sqlAdapter.Fill(myResultSet,"instruments_info");
            return myResultSet;
        }


        // Get list of all instruments
        [HttpGet]
        [Route("instruments/")]
        public IActionResult getInstruments()
        {
            DataSet allInstruments = executeSQL("SELECT * FROM instruments_info");
            return Ok(allInstruments);
        }

        // Get list of all serailized instruments
        [HttpGet]
        [Route("instruments/{type}/")]
        public IActionResult getSerializedInstruments(string type)
        {
            int status = type == "sterilized" ? 1 : 0;
            DataSet allInstruments = executeSQL("SELECT * FROM instruments_info WHERE sterlization_status = " + status);
            return Ok(allInstruments);
        }

        // Update an existing record from the database by instrument id
        [HttpPut]
        [Route("instruments/{id}")]
        public IActionResult updateInstrument(int id )
        {
            try {
                string sqlQuery = "UPDATE instruments_info SET sterlization_status = 0 WHERE i_id =  " + (char)39 + id + (char)39;
                executeSQL(sqlQuery);

                DataSet updatedInstruments = executeSQL("SELECT * FROM instruments_info WHERE i_id = " + (char)39 + id + (char)39);

                int records = updatedInstruments.Tables[0].Rows.Count;
                if (records == 0) {
                    HttpContext.Response.StatusCode = 404;
                    return NotFound("No Instrument found with id  " + id);
                }

                HttpContext.Response.StatusCode = 200;
                var status = "Instrument with Id " + id + " Updated successfully !!";
                
                return Ok(status);

            } catch (Exception e){
               
                HttpContext.Response.StatusCode = 400;
                
                return Problem(
                    detail: e.StackTrace,
                    title: e.Message);
                }
        } 

        // Delete an existing record from the database by instrument id
        [HttpDelete]
        [Route("instruments/{id}")]
        public IActionResult deleteInstrument(int id )
        {
            try {
                    DataSet deletedInstruments = executeSQL("SELECT * FROM instruments_info WHERE i_id = " + (char)39 + id + (char)39);

                    int records = deletedInstruments.Tables[0].Rows.Count;
                    if (records == 0) {
                        HttpContext.Response.StatusCode = 404;
                        return NotFound("No Instrument found with id  " + id);
                    }

                    executeSQL("DELETE FROM instruments_info WHERE i_id = " + (char)39 + id + (char)39);
                    HttpContext.Response.StatusCode = 200;
                    var status = "Instrument with Id " + id + " Deleted successfully !!";
                    
                    return Ok(status);

            } catch (Exception e) {
                HttpContext.Response.StatusCode = 400;
                
                return Problem(
                    detail: e.StackTrace,
                    title: e.Message);
            }
            
        } 

    }

}