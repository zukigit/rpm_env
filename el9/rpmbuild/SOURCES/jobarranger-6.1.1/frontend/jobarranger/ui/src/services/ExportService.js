import http from "../common/http";
import { alertError } from "../components/dialogs/CommonDialog";

const ExportService = {
  exportXML: (data) => {
    return http
      .post(`/exportXML`, data, { responseType: "blob" })
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        alertError("Error", err.message);
        throw err;
      });
  },
};

export default ExportService;
