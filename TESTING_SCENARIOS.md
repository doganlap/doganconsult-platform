# RBAC Testing Scenarios

Follow these scenarios to perform a comprehensive test of the Role-Based Access Control (RBAC) implementation.

---

## Scenario 1: Admin Full Access Verification

**Goal:** Confirm that the default `admin` user has full permissions and can access all features.

1.  **Log in as Admin**:
    *   Ensure you are logged in with the `admin` user. The application should do this automatically in development.

2.  **Test Organization Management**:
    *   Navigate to the **Organizations** page from the main menu.
    *   **VERIFY**: The **"New Organization"** button is visible and enabled.
    *   **VERIFY**: In the data table, the **"Actions"** dropdown is visible for each organization, containing "Edit" and "Delete" options.

3.  **Test Workspace Management**:
    *   Navigate to the **Workspaces** page.
    *   **VERIFY**: The **"New Workspace"** button is visible and enabled.
    *   **VERIFY**: The **"Actions"** dropdown is visible for each workspace.

4.  **Test User Management**:
    *   Navigate to **Administration > Identity Management > Users**.
    *   **VERIFY**: The **"New User"** button is visible and enabled.

*Continue this pattern for any other pages you wish to test. As `admin`, all functionality should be available.*

---

## Scenario 2: Limited "Auditor" Role Verification

**Goal:** Create a new user with view-only permissions and confirm that their access is correctly restricted.

### Part A: Create the Role and User (as Admin)

1.  **Create the "Auditor" Role**:
    *   While logged in as `admin`, navigate to **Administration > Identity Management > Roles**.
    *   Click the **"New Role"** button.
    *   Set the Role Name to `Auditor`.
    *   In the "Permissions" tab that appears, grant **only** the following permissions:
        *   `OrganizationService` -> `Organizations` -> **View**
        *   `AuditService` -> `Audits` -> **View**
        *   `UserProfileService` -> `User Profiles` -> **View**
    *   Ensure all other checkboxes (Create, Edit, Delete) are **unchecked**.
    *   Click **"Save"**.

2.  **Create the "auditor" User**:
    *   Navigate to **Administration > Identity Management > Users**.
    *   Click the **"New User"** button.
    *   Fill in the details:
        *   **User name**: `auditor`
        *   **Email**: `auditor@test.com`
        *   **Password**: `1q2w3E!` (or any password you prefer)
    *   Go to the **"Roles"** tab.
    *   Select the **`Auditor`** role.
    *   Click **"Save"**.

### Part B: Test as the Limited User

1.  **Log in as Auditor**:
    *   Log out of the `admin` account using the user menu in the top-right corner.
    *   Log in with the credentials you just created (`auditor` and the password you set).

2.  **Test Organization Page**:
    *   Navigate to the **Organizations** page.
    *   **VERIFY**: You can see the list of organizations.
    *   **VERIFY**: The **"New Organization"** button is **NOT** visible.
    *   **VERIFY**: The **"Actions"** dropdown menu is **NOT** visible in the table.

3.  **Test Audit Log Page**:
    *   Navigate to the **Audit Logs** page.
    *   **VERIFY**: You can see the audit log entries.

4.  **Test Forbidden Page**:
    *   Attempt to navigate to the **Workspaces** page from the main menu.
    *   **VERIFY**: The page shows an **"Authorization Failed"** message, because the `Auditor` role has no permissions for the Workspace service.

---

Completing these two scenarios will confirm that the RBAC system is working correctly for both permissive and restrictive cases.
