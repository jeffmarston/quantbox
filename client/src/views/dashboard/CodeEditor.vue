<template>
  <div class="animated">
    <b-row>
      <b-col xl="12" class="no-pad" >
        <codemirror ref="myCm" :value="code" :options="cmOptions"></codemirror>
      </b-col>
    </b-row>
  </div>
</template>

<script>
// const _ = require("lodash");
// require component and styles
import { codemirror } from "vue-codemirror";
import "codemirror/lib/codemirror.css";
import "codemirror/mode/javascript/javascript.js";
// theme css
import "codemirror/theme/monokai.css";
import "codemirror/theme/base16-dark.css";
import "codemirror/theme/darcula.css";
import "codemirror/theme/twilight.css";
import { setTimeout } from "timers";

export default {
  name: "code-editor",
  components: { codemirror },
  mounted() {
    setTimeout(() => {
      this.$refs.myCm.refresh();
    }, 100);
  },
  data: () => {
    return {
      myCm: {},
      code: `using RealTick.Api.Communication;
using RealTick.Api.Data;
using RealTick.Api.Talipc;
using System;
using System.Collections.Generic;

public EmsAdapter(EmsSettings settings)
{
    Settings = settings;
    if (_app == null)
        _app = new TalipcToolkitApp();

    Service = "ACCOUNT_GATEWAY";
    Topic = "ORDER";

    // Set up Connection
    _query = _app.GetAsyncQuery(settings.Gateway, Service, Topic);
    _query.OnTerminate += OnTerminate;
    _query.OnOtherAck += OnOtherAck;
    _query.OnAdviseData += OnAdviseData;
    _query.OnExecuteAck += OnExecuteAck;
    _query.OnRequestData += OnRequestData;

    if (!_query.Connect())
    {
        Console.WriteLine("Unable to connect");
    }
    else
    {
        _query.Advise("ORDERS;*;", "TAL4");
        _query.Request("ORDERS;*;", "TAL4");
    }
}

#region Callbacks
private void OnRequestData(object sender, DataEventArgs e)
{
    string sItem = e.Item;
    Console.WriteLine("OnRequestData: " + sItem);

    if (sItem.StartsWith("ORDERS;"))
    {
        IDataBlock block = e.Data.GetDataAsBlock();
        ProcessDataBlock(block, true);
    }
}

private void OnAdviseData(object sender, DataEventArgs e)
{
    //Console.WriteLine("OnAdviseData");
    string sItem = e.Item;
    if (sItem.StartsWith("ORDERS;"))
    {
        IDataBlock block = e.Data.GetDataAsBlock();
        ProcessDataBlock(block, false);
        RecalculateStats();
    }
}

private void ProcessDataBlock(IDataBlock block, bool bRequest)
{
    foreach (Row row in block)
    {
        OrderRecord order = new OrderRecord();
        foreach (Field f in row)
        {
            if (f.FieldInfo.Name == "TYPE")
                order.Type = f.StringValue;
            else if (f.FieldInfo.Name == "ORDER_ID")
                order.OrderID = f.StringValue;
            else if (f.FieldInfo.Name == "CURRENT_STATUS")
                order.Status = f.StringValue;
            else if (f.FieldInfo.Name == "DISP_NAME")
                order.Symbol = f.StringValue;
            else if (f.FieldInfo.Name == "VOLUME")
                order.lQty = f.IntValue;
            else if (f.FieldInfo.Name == "VOLUME_TRADED")
                order.lQtyTraded = f.IntValue;
            else if (f.FieldInfo.Name == "BUYORSELL")
                order.Side = f.StringValue;
            else if (f.FieldInfo.Name == "AVG_PRICE")
                order.dPrice = f.DoubleValue;
            else if (f.FieldInfo.Name == "WORKING_QTY")
                order.lWorking = f.LongValue;
            else if (f.FieldInfo.Name == "ORDER_TAG")
                order.OrderTag = f.StringValue;
        }
        ProcessOrder(order, bRequest);
    }
}

private void ProcessOrder(OrderRecord order, bool bRequest)
{
    if (order != null)
    {
        if (order.Type == "UserSubmitStagedOrder")
        {
            string sDetails = order.GetDetails();
            if (!bRequest)    // don't want to spam the console at startup...
                Console.WriteLine("Received Order Update: " + sDetails);
            _book[order.OrderID] = order;
        }
    }
}
`,
      cmOptions: {
        tabSize: 4,
        mode: "text/javascript",
        theme: 'darcula',
        //theme: 'twilight',
        //theme: "monokai",
        //theme: "default",
        lineNumbers: true,
        line: true,
        height: 500
      }
    };
  },
  methods: {
    populateAlgo() {}
  }
};
</script>

<style>
.no-pad {
  padding:0;
}
.form-row {
  margin-bottom: 2px;
}
.CodeMirror {
  height: 400px;
}
</style>
