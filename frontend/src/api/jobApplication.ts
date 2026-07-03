export interface JobApplication {
  id: string;
  company: string;
  position: string;
  status: string;
  appliedOn: string;
  description?: string;
}

export async function getJobApplications(): Promise<JobApplication[]> {
  const response = await fetch("/api/v1/applications");

  if (!response.ok) {
    throw new Error(`Failed to load applications: ${response.status}`);
  }

  return response.json();
}

export async function getJobApplicationById(
  id: string,
): Promise<JobApplication | null> {
  const response = await fetch(`/api/v1/applications/${id}`);

  if (response.status === 404) {
    return null;
  }

  if (!response.ok) {
    throw new Error(
      `Failed to load application with id ${id}: ${response.status}`,
    );
  }

  return response.json();
}
