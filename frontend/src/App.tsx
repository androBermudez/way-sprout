import { BrowserRouter, Routes, Route } from "react-router-dom"

import { ApplicationsPage } from "@/pages/ApplicationsPage"
import { ApplicationDetailsPage } from "@/pages/ApplicationDetailsPage"
import { HomePage } from "@/pages/HomePage"
import { ApplicationsLayout } from "./pages/ApplicationsLayout"

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<HomePage />} index={true} />
        <Route path="/applications" element={<ApplicationsLayout />}>
          <Route index element={<ApplicationsPage />} />
          <Route path=":id" element={<ApplicationDetailsPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  )
}

export default App
