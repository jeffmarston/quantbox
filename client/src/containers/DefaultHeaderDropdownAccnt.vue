<template>
  <AppHeaderDropdown right no-caret>
    <template slot="header">
          <i class="icon-options-vertical" style="font-size: 18px;"></i>
    </template>\
    <template slot="dropdown">
      <b-dropdown-header
        tag="div"
        class="text-center">
        <strong>Settings</strong>
      </b-dropdown-header>
      <b-dropdown-item><i class="icon-user" /> Permissions</b-dropdown-item>
      <b-dropdown-item><router-link to="/settings"><i class="icon-settings" /> Settings</router-link></b-dropdown-item>
      <b-dropdown-item><i class="icon-note" /> Audit Trail</b-dropdown-item>
      <b-dropdown-divider />
      <b-dropdown-item><i class="icon-lock" /> Logout</b-dropdown-item>
    </template>
  </AppHeaderDropdown>
</template>

<script>
import { HeaderDropdown as AppHeaderDropdown } from '@coreui/vue'
export default {
  name: 'DefaultHeaderDropdownAccnt',
  components: {
    AppHeaderDropdown
  },
  data: () => {
    return { itemsCount: 42 }
  }
}
</script>
