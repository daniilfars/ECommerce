import { BrowserRouter } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import AppRouter from "./router/AppRouter";
import Header from "./components/layout/Header";
import "./App.css";

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Header />
        <AppRouter />
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;