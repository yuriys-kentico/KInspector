import axios from "axios";

class ReportService {
  getReports(instanceGuid) {
    return new Promise((resolve) => {
      axios
        .get(`/api/reports/${instanceGuid}`)
        .then((r) => r.data)
        .then((reports) => {
          resolve(reports);
        });
    });
  }

  getReportResults({ codeName, instanceGuid }) {
    return new Promise((resolve) => {
      axios
        .get(`/api/reports/${codeName}/results/${instanceGuid}`)
        .then((r) => r.data)
        .then((results) => {
          resolve(results);
        });
    });
  }
}

export const reportService = new ReportService();