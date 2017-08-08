using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abacus.DAL;

namespace SEPAFileManager
{
    class Checker
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal static void ACKCheck(string Table, DAO objDAO)
        {
            try
            {
                //Check 1 week back
                string SearchSQL = @"SELECT * FROM " + Table +
                    " WHERE (ACK IS NULL OR ACK = 'NAK') AND SENT = 1 AND (DATEDIFF(hour, ServerDate, GETDATE()) > 24  AND DATEDIFF(hour, ServerDate, GETDATE()) < 168)";
                System.Data.DataTable returnDT = objDAO.Search(SearchSQL);
                System.Data.DataRow[] NoACK = returnDT.Select("ACK IS NULL");
                System.Data.DataRow[] NAK = returnDT.Select("ACK = 'NAK'");
                string ErrorMsg = string.Empty; ;

                if (returnDT.Rows.Count > 0)
                {
                    if (NoACK.Length > 0)
                        ErrorMsg = (String.Concat(DateTime.Now, Table, String.Format("   {0} Logs with no ACK", NoACK.Length)));
                    if (NAK.Length > 0)
                        ErrorMsg = (String.Concat(ErrorMsg, Table, String.Format("    {0} Logs with NAK", NAK.Length)));

                    log.Error(ErrorMsg);
                }
            }
            catch (Exception ex)
            {
                log.Error(String.Concat(DateTime.Now, ", ACKCheck ERROR - Check ACK has failed", ex.Message));
                throw;
            }
        }

        internal static void CT_UnsentCheck(DAO objDAO)  //CTs, also add returns..
        {
            try
            {
                string SearchSQL = @"SELECT * FROM SEPA_CT WHERE CTOut = 1 AND SEPA_LogID IS NULL AND ReceiptNo > 0
                                    AND DATEDIFF(day, IntrBkSttlmDt, DATEADD(day,-7,GETDATE())) < 0
                                    AND (SELECT TOP 1 T.ReversalReceiptNo FROM CUTransaction T WHERE T.ReceiptNo = SEPA_CT.ReceiptNo) = 0";
                System.Data.DataTable returnDT = objDAO.Search(SearchSQL);
                string ErrorMsg = string.Empty; ;

                if (returnDT.Rows.Count > 0)
                {
                    ErrorMsg = (String.Concat(DateTime.Now, String.Format("   {0} Unsent CTs", returnDT.Rows.Count)));
                    log.Error(ErrorMsg);
                }
            }
            catch (Exception ex)
            {
                log.Error(String.Concat(DateTime.Now, ", CT_UnsentCheck ERROR - Check Unsent CTs has failed", ex.Message));
                throw;
            }
        }

        internal static void CVFCheck(DAO objDAO)
        {
            try
            {
                string SearchSQL = @"SELECT * FROM SEPA_CTLog WHERE CTOut = 1 AND NbOfTxs > 0 AND (CVF_GrpSts <> 'ACCP' OR CVF_GrpSts IS NULL) 
                    AND (DATEDIFF(hour, ServerDate, GETDATE()) > 24  AND DATEDIFF(hour, ServerDate, GETDATE()) < 168)";
                System.Data.DataTable returnDT = objDAO.Search(SearchSQL);
                System.Data.DataRow[] NoCVF = returnDT.Select("CVF_GrpSts IS NULL");
                System.Data.DataRow[] NoACCP = returnDT.Select("CVF_GrpSts <> 'ACCP'");
                string ErrorMsg = string.Empty; ;

                if (returnDT.Rows.Count > 0)
                {
                    if (NoCVF.Length > 0)
                        ErrorMsg = (String.Concat(DateTime.Now, String.Format("   {0} Logs with no CVF", NoCVF.Length)));
                    if (NoACCP.Length > 0)
                        ErrorMsg = (String.Concat(ErrorMsg, String.Format("    {0} Logs with non ACCP", NoACCP.Length)));

                    log.Error(ErrorMsg);
                }
            }
            catch (Exception ex)
            {
                log.Error(String.Concat(DateTime.Now, ", CVFCheck ERROR - Check CVF has failed", ex.Message));
                throw;
            }
        }

        internal static void DVFCheck(DAO objDAO)
        {
            try
            {
                string SearchSQL = @"SELECT * FROM SEPA_DDLog WHERE DDOut = 1 AND NbOfTxs > 0 AND (DVF_GrpSts <> 'ACCP' OR DVF_GrpSts IS NULL) 
                    AND (DATEDIFF(hour, ServerDate, GETDATE()) > 24  AND DATEDIFF(hour, ServerDate, GETDATE()) < 168)";
                System.Data.DataTable returnDT = objDAO.Search(SearchSQL);
                System.Data.DataRow[] NoDVF = returnDT.Select("DVF_GrpSts IS NULL");
                System.Data.DataRow[] NoACCP = returnDT.Select("DVF_GrpSts <> 'ACCP'");
                string ErrorMsg = string.Empty; ;

                if (returnDT.Rows.Count > 0)
                {
                    if (NoDVF.Length > 0)
                        ErrorMsg = (String.Concat(DateTime.Now, String.Format("   {0} Logs with no DVF", NoDVF.Length)));
                    if (NoACCP.Length > 0)
                        ErrorMsg = (String.Concat(ErrorMsg, String.Format("    {0} Logs with non ACCP", NoACCP.Length)));

                    log.Error(ErrorMsg);
                }
            }
            catch (Exception ex)
            {
                log.Error(String.Concat(DateTime.Now, ", DVFCheck ERROR - Check DVF has failed", ex.Message));
                throw;
            }
        }
    }
}
