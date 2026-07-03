export interface JobApplication {
  id: string;
  company: string;
  position: string;
  status: string;
  appliedOn: string;
}

export async function getJobApplications(): Promise<JobApplication[]> {
  const response = await fetch("/api/v1/applications");

  if (!response.ok) {
    throw new Error(`Failed to load applications: ${response.status}`);
  }

  return response.json();
}
