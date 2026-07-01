import { BrowserRouter } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import AppRouter from "./router/AppRouter";
import Header from "./components/layout/Header";
import Footer from "./components/layout/Footer";
import "./App.css";

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Header />
        <main style={{ flex: 1 }}>
          <AppRouter />
        </main>
        <Footer />
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;