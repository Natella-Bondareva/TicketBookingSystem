import axios from 'axios';

export const searchRoutesApi = async (dto) => {
  try {
    const response = await axios.post('http://localhost:5086/api/Search/with-availability', dto);
    return response.data;
  } catch (error) {
    console.error("🚨 Axios Error:", error);
    if (error.response) {
      console.log("📡 Server responded:", error.response.status, error.response.data);
    } else if (error.request) {
      console.log("📭 No response received:", error.request);
    } else {
      console.log("🧠 Request config error:", error.message);
    }
    throw error;
  }
};
