import http from '../common/http'

const JobExecutionResultService = {
    getAllUserAlias: (data={}) => {
        return http.post(`/jobExecutionResult/getUsers`,data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("Auth service err", err);
                throw err
            })
    },
    getJobExecutionResult: (data) => {
        return http.post(`/jobExecutionResult/search`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("Auth service err", err);
                throw err
            })
    },
    getExportCsv: (data) => {
        return http.post(`/jobExecutionResult/exportCsv`, data,{ responseType: "blob" })
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("Auth service err", err);
                throw err
            })
    },
};

export default JobExecutionResultService;