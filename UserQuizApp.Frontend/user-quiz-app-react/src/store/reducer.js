const initialState = {
  isWrapAdmin: false,
  list: [],
  wrapName: null,
};

const rootReducer = (state = initialState, action) => {
  switch (action.type) {
    case "UPDATE_DATA":
      return action.payload;
    case "UPDATE_LIST":
      return { ...state, list: action.payload };
    default:
      return state;
  }
};

export default rootReducer;
