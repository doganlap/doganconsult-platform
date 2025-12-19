# RBAC CRUD Testing Scenarios (Create, Read, Update, Delete)

Follow these scenarios to perform a detailed test of the Create, Read, Update, and Delete (CRUD) permissions.

---

## Scenario 1: Admin Full CRUD Access Verification

**Goal:** Confirm that the `admin` user can perform all CRUD operations on the Organizations module.

1.  **Log in as Admin**:
    *   Ensure you are logged in with the `admin` user.

2.  **Navigate to Organizations**:
    *   Go to the **Organizations** page from the main menu.

3.  **CREATE Operation**:
    *   Click the **"New Organization"** button.
    *   In the modal dialog, enter a **Display Name**, for example: `Test Corp`.
    *   Click **"Save"**.

4.  **READ Operation**:
    *   **VERIFY**: The new organization `Test Corp` appears in the list. This confirms the Read (View) permission is working.

5.  **UPDATE Operation**:
    *   Find `Test Corp` in the list.
    *   Click the **"Actions"** dropdown next to it and select **"Edit"**.
    *   In the modal dialog, change the **Display Name** to `Test Corp - Edited`.
    *   Click **"Save"**.
    *   **VERIFY**: The organization's name in the list updates to `Test Corp - Edited`.

6.  **DELETE Operation**:
    *   Find `Test Corp - Edited` in the list.
    *   Click the **"Actions"** dropdown next to it and select **"Delete"**.
    *   A confirmation dialog will appear. Click **"Yes"**.
    *   **VERIFY**: The organization `Test Corp - Edited` is removed from the list.

**Conclusion**: If you can complete all these steps, it confirms the `admin` user has full Create, Read, Update, and Delete permissions for the Organizations module.

---

## Scenario 2: Limited "Auditor" Role CRUD Restriction

**Goal:** Confirm that the `auditor` user (with only View permissions) is correctly blocked from all Create, Update, and Delete operations.

1.  **Log in as Auditor**:
    *   If you are logged in as `admin`, log out.
    *   Log in with the `auditor` user credentials (`username: auditor`, `password: 1q2w3E!`).

2.  **Navigate to Organizations**:
    *   Go to the **Organizations** page from the main menu.

3.  **Verify NO CREATE**:
    *   **VERIFY**: The **"New Organization"** button is **NOT** visible anywhere on the page.

4.  **Verify NO UPDATE or DELETE**:
    *   Look at the table of organizations.
    *   **VERIFY**: The **"Actions"** column/dropdown is completely hidden. There are no "Edit" or "Delete" options available.

5.  **Verify READ is still possible**:
    *   **VERIFY**: You can still see the list of organizations, confirming the View/Read permission is working correctly.

**Conclusion**: If the buttons and actions for Create, Update, and Delete are missing, it confirms that the RBAC system is successfully restricting the `auditor` user based on their assigned permissions.
