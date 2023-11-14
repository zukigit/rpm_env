import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';

const initialState = {
    data: {},
    responseData :{},
}

export const FormObjectSlice = createSlice({
  name: 'formObject',
  initialState,
  reducers: {
    setFormData:{
      reducer: (state,action) =>{
        if(action.payload){
          state.data = action.payload;
        }else{
            state.data = [];
        }
      },
      prepare: (formData) =>{
        return {payload:formData};
      }
    },
  },
  extraReducers(builder) {
  }
})
export const { setFormData } = FormObjectSlice.actions
export default FormObjectSlice.reducer
