import http from '../common/http'

const JobnetService = {
    getAvailableHosts: (data = {}) => {
        return http.post(`/jobnet/host`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    getDefineValueJobCon: (data = {}) => {
        return http.post(`/jobnet/defineValueJobCon`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    getDefineExtendedJob: (data = {}) => {
        return http.post(`/jobnet/defineExtendedJob`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    getAllJobnet: (data = {}) => {
        return http.post(`/jobnet/all`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    getAllCalendar: (data = {}) => {
        return http.post(`/calendar/calendarList`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    getValidOrLatestCalendar: (data = {}) => {
        return http.post(`/calendar/getValidOrLatest`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    getHostGroup: (data = {}) => {
        return http.post(`/jobnet/hostGroup`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
        getHostByZabbixApi: (data) => {
        return http.post(`/jobnet/zabbixApi/host`,data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    getItemByZabbixApi: (data) => {
        return http.post(`/jobnet/zabbixApi/item`,data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    getTriggerByZabbixApi: (data) => {
        return http.post(`/jobnet/zabbixApi/trigger`,data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    getJobnetOption: (data) => {
        return http.post(`/jobnet/getJobnetOption`,data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    initCreateJobnet: (data) => {
        return http.post(`/jobnet/initCreate`,data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("Auth service err", err);
                throw err
            })
    },
    initEditJobnet: (data) => {
        return http.post(`/jobnet/initEdit`,data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("Auth service err", err);
                throw err
            })
    },
    initJobnetIcon: (data) => {
        return http.post(`/jobnet/initSubJobnet`,data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("Auth service err", err);
                throw err
            })
    },
    saveJobnet: (data) => {
        return http.post('/jobnet/save', data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("Auth service err", err);
                throw err
            })
    },
    checkJobnet: (data) => {
        return http.post('/jobnet/check', data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("Auth service err", err);
                throw err
            })
    },
    checkJob: (data) => {
        return http.post('/jobnet/checkJob', data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("Auth service err", err);
                throw err
            })
    },
    getJobnetInfo: (data) => {
        return http.post('/jobnet/get', data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("Auth service err", err);
                throw err
            })
    },
};

export default JobnetService;