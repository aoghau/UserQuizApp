const initialState = {
  list: [],
  wrapName: null,
  quizList: [],
};

const rootReducer = (state = initialState, action) => {
  switch (action.type) {
    case "UPDATE_DATA":
      return action.payload;
    case "UPDATE_LIST":
      return {
        ...state,
        list: action.payload.list,
        wrapName: action.payload.wrapName,
      };
    case "UPDATE_QUIZLIST":
      return { ...state, quizList: action.payload };
    default:
      return state;
  }
};

export default rootReducer;
