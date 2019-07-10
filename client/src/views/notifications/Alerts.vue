<template>
  <div class="wrapper">
    <div class="animated fadeIn">
      <b-row>
        <b-col cols="12" md="12">
            <div>
              <b-alert show dismissible variant="danger">
                3 Trades exist with invalid Action
              </b-alert>
              <b-alert show dismissible variant="danger">
                42 Orphaned fills with no Trade
              </b-alert>
              <b-alert show dismissible variant="danger">
                2 Invalid SecTypes
              </b-alert>
            </div>
        </b-col>
      </b-row>
    </div>
  </div>
</template>

<script>
export default {
  name: 'alerts',
  data () {
    return {
      dismissSecs: 10,
      dismissCountDown: 0,
      showDismissibleAlert: false
    }
  },
  methods: {
    countDownChanged (dismissCountDown) {
      this.dismissCountDown = dismissCountDown
    },
    showAlert () {
      this.dismissCountDown = this.dismissSecs
    }
  }
}
</script>
