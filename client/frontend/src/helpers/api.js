import axios from "@/helpers/axios";

const GATEWAY = "http://localhost:7000";



export const login = async (username, password) => {
  return axios.post(`${GATEWAY}/auth/login`, {
    username,
    password
  });
};


export const register = async (username, email, password) =>
  axios.post(`${GATEWAY}/auth/register`, {
    username,
    email,
    password
  });

export const createShortUrl = (url, alias) =>
  axios.post(`${GATEWAY}/shorten/create`, {
    url,
    alias
  });

export const getHistory = () =>
  axios.get(`${GATEWAY}/shorten/history`);

export const deleteUrl = (id) =>
  axios.delete(`${GATEWAY}/shorten/delete/${id}`)

export const editUrl = (oldId, newId) =>
  axios.post(`${GATEWAY}/shorten/edit`, {
    oldId,
    newId
  })

export const logout = () => {
  localStorage.removeItem("token");
  localStorage.removeItem("currentUser");
};
