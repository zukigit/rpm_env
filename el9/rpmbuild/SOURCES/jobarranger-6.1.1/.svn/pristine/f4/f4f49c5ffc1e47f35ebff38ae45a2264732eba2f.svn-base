import http from '../common/http'

const JobExecutionManagementService = {
    getAllOperationList: (data={}) => {
        return http.post(`/jobExecManagement/all`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    getErrorOperationList: (data={}) => {
        return http.post(`/jobExecManagement/error`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    getDuringOperationList: (data={}) => {
        return http.post(`/jobExecManagement/during`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
};

export default JobExecutionManagementService;