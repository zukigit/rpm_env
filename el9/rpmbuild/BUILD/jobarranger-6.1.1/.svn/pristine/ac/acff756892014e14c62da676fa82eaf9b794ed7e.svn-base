import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";

const initialState = {
  responseData : [],
};

export const ResponseSlice = createSlice({
  name: "responseData",
  initialState,
  reducers: {
    setResponseData: {
      reducer: (state, action) => {
        if (action.payload) {
          state.responseData = action.payload;
        }
      },
      prepare: (responseData) => {
        return { payload: responseData };
      },
    },
  },

//   extraReducers(builder) {
    
//   },
});
export const { setResponseData } =
ResponseSlice.actions;
export default ResponseSlice.reducer;
