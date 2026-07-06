import { Outlet } from "react-router-dom"

export function ApplicationsLayout() {
  return (
    <div className="p-6">
      <h1 className="text-2xl font-semibold mb-4">Applications</h1>
      <Outlet />
    </div>
  )
}
